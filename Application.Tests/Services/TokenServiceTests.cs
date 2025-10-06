using Application.Exceptions;
using Application.Services;
using Application.Services.Interfaces;
using Application.Tests.Extenstions;
using AutoFixture;
using Configuration.Options;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Application.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Fixture _fixture;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOptions<JwtTokenOptions> _jwtTokenOptions;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IPriciplesFromTokenProvider _priciplesFromTokenProvider;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _fixture = new Fixture().FixCircularReference();
            _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _jwtTokenOptions = Substitute.For<IOptions<JwtTokenOptions>>();
            _jwtSecurityTokenHandler = Substitute.For<JwtSecurityTokenHandler>();
            _priciplesFromTokenProvider = Substitute.For<IPriciplesFromTokenProvider>();

            var jwtTokenOptions = new JwtTokenOptions
            {
                Key = "test-key",
                Issuer = "test-issuer",
                Audience = "test-audience",
                AccessTokenExpirationMinutes = 30,
                RefreshTokenExpirationDays = 7
            };
            _jwtTokenOptions.Value.Returns(jwtTokenOptions);

            _tokenService = new TokenService(_refreshTokenRepository, _userRepository, _jwtTokenOptions, _jwtSecurityTokenHandler, _priciplesFromTokenProvider);
        }

        [Fact]
        public async Task GenerateTokensAsync_ShouldGenerateAndSaveTokensForValidUser()
        {
            // Arrange
            var user = _fixture.Create<UserDao>();

            var expectedAccessToken = "mock-access-token";

            _jwtSecurityTokenHandler.WriteToken(Arg.Any<JwtSecurityToken>()).Returns(expectedAccessToken);
            _refreshTokenRepository.InsertTokenAsync(Arg.Any<RefreshTokenDao>()).Returns(Task.CompletedTask);
            _refreshTokenRepository.SaveChangesAsync().Returns(Task.CompletedTask);

            // Act
            var authTokens = await _tokenService.GenerateTokensAsync(user);

            // Assert
            authTokens.Should().NotBeNull();
            authTokens.AccessToken.Should().Be(expectedAccessToken);
            authTokens.RefreshToken.Should().NotBeNullOrEmpty(); // Generated as part of GenerateRefreshToken()
            authTokens.AccessTokenExpiry.Should().BeAfter(DateTime.UtcNow);
            authTokens.RefreshTokenExpiry.Should().BeAfter(DateTime.UtcNow);

            // Verify repository and handler interactions
            await _refreshTokenRepository.Received(1).InsertTokenAsync(Arg.Any<RefreshTokenDao>());
            await _refreshTokenRepository.Received(1).SaveChangesAsync();
            _jwtSecurityTokenHandler.Received(1).WriteToken(Arg.Any<JwtSecurityToken>());
        }

        [Fact]
        public async Task RefreshTokensAsync_ShouldGenerateNewTokens_WhenTokensAreValid()
        {
            // Arrange
            var userId = 1;
            var accessToken = "mock-valid-access-token";
            var refreshToken = "valid-refresh-token";
            var newAccessToken = "new-access-token";

            var existingRefreshTokenDao = _fixture.Build<RefreshTokenDao>()
                .With(t => t.Token, refreshToken)
                .With(t => t.IsUsed, false)
                .With(t => t.IsRevoked, false)
                .With(t => t.Expires, DateTime.UtcNow.AddDays(3))
                .Create();

            var user = _fixture.Build<UserDao>()
                .With(u => u.Id, userId)
                .With(u => u.RefreshTokens, new List<RefreshTokenDao> { existingRefreshTokenDao })
                .Create();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            _priciplesFromTokenProvider.GetPrincipalFromExpiredToken(accessToken).Returns(claimsPrincipal);
            _userRepository.GetByIdAsync(userId).Returns(user);
            _refreshTokenRepository.SaveChangesAsync().Returns(Task.CompletedTask);
            _jwtSecurityTokenHandler.WriteToken(Arg.Any<JwtSecurityToken>()).Returns(newAccessToken);

            // Act
            var authTokens = await _tokenService.RefreshTokensAsync(accessToken, refreshToken);

            // Assert
            authTokens.Should().NotBeNull();
            authTokens.AccessToken.Should().Be(newAccessToken);
            authTokens.RefreshToken.Should().NotBeNullOrEmpty();
            authTokens.AccessTokenExpiry.Should().BeAfter(DateTime.UtcNow);
            authTokens.RefreshTokenExpiry.Should().BeAfter(DateTime.UtcNow);

            // Verify refresh token updates
            var savedToken = user.RefreshTokens.First();
            savedToken.IsUsed.Should().BeTrue();

            // Verify repository and handler interactions
            await _refreshTokenRepository.Received(1).SaveChangesAsync();
            _priciplesFromTokenProvider.Received(1).GetPrincipalFromExpiredToken(accessToken);
        }

        [Fact]
        public async Task RefreshTokensAsync_ShouldThrowException_WhenRefreshTokenIsInvalid()
        {
            // Arrange
            var userId = 1;
            var existingRefreshTokenDao = _fixture.Build<RefreshTokenDao>()
                .With(t => t.IsRevoked, false)
                .Create();

            var user = _fixture.Build<UserDao>()
                .With(u => u.Id, userId)
                .With(u => u.RefreshTokens, new List<RefreshTokenDao> { existingRefreshTokenDao })
                .Create();

            var accessToken = "mock-valid-access-token";
            var refreshToken = "revoked-token";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            _priciplesFromTokenProvider.GetPrincipalFromExpiredToken(accessToken).Returns(claimsPrincipal);
            _userRepository.GetByIdAsync(userId).Returns(user);

            // Act
            Func<Task> act = async () => await _tokenService.RefreshTokensAsync(accessToken, refreshToken);

            // Assert
            await act.Should()
                .ThrowAsync<BadArgumentException>()
                .WithMessage("Access or refresh token are invalid.");

            // Verify interactions
            await _userRepository.Received(1).GetByIdAsync(userId);
            await _refreshTokenRepository.DidNotReceive().SaveChangesAsync();
            _priciplesFromTokenProvider.Received(1).GetPrincipalFromExpiredToken(accessToken);
        }

        [Fact]
        public async Task RefreshTokensAsync_ShouldThrowException_WhenAccessTokenIsInvalid()
        {
            // Arrange
            var accessToken = "invalid-access-token";
            var refreshToken = "valid-refresh-token";

            _priciplesFromTokenProvider.GetPrincipalFromExpiredToken(accessToken).Returns((ClaimsPrincipal)null);

            // Act
            Func<Task> act = async () => await _tokenService.RefreshTokensAsync(accessToken, refreshToken);

            // Assert
            await act.Should()
                .ThrowAsync<BadArgumentException>()
                .WithMessage("Access or refresh token are invalid.");

            // Verify interactions
            await _userRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>());
            await _refreshTokenRepository.DidNotReceive().SaveChangesAsync();
        }
    }
}

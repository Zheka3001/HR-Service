using Application.Services;
using Application.Services.Interfaces;
using Configuration.Options;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IOptions<JwtTokenOptions>> _jwtTokenOptionsMock;
        private readonly Mock<JwtSecurityTokenHandler> _jwtSecurityTokenHandlerMock;
        private readonly Mock<IPriciplesFromTokenProvider> _priciplesFromTokenProviderMock;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtTokenOptionsMock = new Mock<IOptions<JwtTokenOptions>>();
            _jwtSecurityTokenHandlerMock = new Mock<JwtSecurityTokenHandler>();
            _priciplesFromTokenProviderMock = new Mock<IPriciplesFromTokenProvider>();

            var jwtTokenOptions = new JwtTokenOptions
            {
                Key = "test-key",
                Issuer = "test-issuer",
                Audience = "test-audience",
                AccessTokenExpirationMinutes = 30,
                RefreshTokenExpirationDays = 7
            };
            _jwtTokenOptionsMock.Setup(opt => opt.Value).Returns(jwtTokenOptions);

            _tokenService = new TokenService(
                _refreshTokenRepositoryMock.Object,
                _userRepositoryMock.Object,
                _jwtTokenOptionsMock.Object,
                _jwtSecurityTokenHandlerMock.Object,
                _priciplesFromTokenProviderMock.Object);
        }

        [Fact]
        public async Task GenerateTokensAsync_ShouldGenerateAndSaveTokensForValidUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Role = Role.Admin
            };

            var expectedAccessToken = "mock-access-token";

            _jwtSecurityTokenHandlerMock
                .Setup(handler => handler.WriteToken(It.IsAny<JwtSecurityToken>()))
                .Returns(expectedAccessToken);

            _refreshTokenRepositoryMock
                .Setup(repo => repo.InsertTokenAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _refreshTokenRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var authTokens = await _tokenService.GenerateTokensAsync(user);

            // Assert
            authTokens.Should().NotBeNull();
            authTokens.AccessToken.Should().Be(expectedAccessToken);
            authTokens.RefreshToken.Should().NotBeNullOrEmpty(); // Generated as part of GenerateRefreshToken()
            authTokens.AccessTokenExpiry.Should().BeAfter(DateTime.UtcNow);
            authTokens.RefreshTokenExpiry.Should().BeAfter(DateTime.UtcNow);

            // Verify repository and handler interactions
            _refreshTokenRepositoryMock.Verify(repo => repo.InsertTokenAsync(It.IsAny<RefreshToken>()), Times.Once);
            _refreshTokenRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);

            _jwtSecurityTokenHandlerMock.Verify(handler => handler.WriteToken(It.IsAny<JwtSecurityToken>()), Times.Once);
        }

        [Fact]
        public async Task RefreshTokensAsync_ShouldGenerateNewTokens_WhenTokensAreValid()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Role = Role.Admin,
                RefreshTokens = new List<RefreshToken>
                    {
                        new RefreshToken
                        {
                            Token = "valid-refresh-token",
                            IsUsed = false,
                            IsRevoked = false,
                            Expires = DateTime.UtcNow.AddDays(3)
                        }
                    }
            };

            var accessToken = "mock-valid-access-token";
            var refreshToken = "valid-refresh-token";
            var newAccessToken = "new-access-token";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            _priciplesFromTokenProviderMock
                .Setup(provider => provider.GetPrincipalFromExpiredToken(accessToken))
                .Returns(claimsPrincipal);

            _userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _refreshTokenRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _jwtSecurityTokenHandlerMock
                .Setup(handler => handler.WriteToken(It.IsAny<JwtSecurityToken>()))
                .Returns(newAccessToken);

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
            _refreshTokenRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _priciplesFromTokenProviderMock.Verify(provider => provider.GetPrincipalFromExpiredToken(accessToken), Times.Once);
        }

        [Fact]
        public async Task RefreshTokensAsync_ShouldThrowException_WhenRefreshTokenIsInvalid()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                RefreshTokens = new List<RefreshToken>
                {
                    new RefreshToken
                    {
                        Token = "revoked-token",
                        IsUsed = false,
                        IsRevoked = true, // Token is revoked
                        Expires = DateTime.UtcNow.AddDays(5)
                    }
                }
            };

            var accessToken = "mock-valid-access-token";
            var refreshToken = "revoked-token";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            _priciplesFromTokenProviderMock
                .Setup(provider => provider.GetPrincipalFromExpiredToken(accessToken))
                .Returns(claimsPrincipal);

            _userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            Func<Task> act = async () => await _tokenService.RefreshTokensAsync(accessToken, refreshToken);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Access or refresh token are invalid.");

            // Verify interactions
            _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
            _refreshTokenRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
            _priciplesFromTokenProviderMock.Verify(provider => provider.GetPrincipalFromExpiredToken(accessToken), Times.Once);
        }

        [Fact]
        public async Task RefreshTokensAsync_ShouldThrowException_WhenAccessTokenIsInvalid()
        {
            // Arrange
            var accessToken = "invalid-access-token";
            var refreshToken = "valid-refresh-token";

            _priciplesFromTokenProviderMock
                .Setup(provider => provider.GetPrincipalFromExpiredToken(accessToken))
                .Returns((ClaimsPrincipal)null);

            // Act
            Func<Task> act = async () => await _tokenService.RefreshTokensAsync(accessToken, refreshToken);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Access or refresh token are invalid.");

            // Verify interactions
            _userRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _refreshTokenRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
    }
}

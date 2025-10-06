using Application.Services.Interfaces;
using Application.Services;
using Application.Models;
using DataAccessLayer.Models;
using Xunit;
using FluentAssertions;
using DataAccessLayer.Repositories.Interfaces;
using Application.Exceptions;
using AutoFixture;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using FluentValidation;
using Application.Tests.Extenstions;
using System.Security.Cryptography;
using System.Text;

namespace Application.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Fixture _fixture;
        private readonly IValidationService _validationService;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _fixture = new Fixture().FixCircularReference();
            _tokenService = Substitute.For<ITokenService>();
            _validationService = Substitute.For<IValidationService>();
            _userRepository = Substitute.For<IUserRepository>();

            _authService = new AuthService(_userRepository, _tokenService, _validationService);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnAuthTokens_WhenLoginIsValid()
        {
            // Arrange
            var validEmail = "test@example.com";
            var validPassword = _fixture.Create<string>();
            var passwordSalt = _fixture.Create<byte[]>();
            var hashedPassword = new HMACSHA512(passwordSalt).ComputeHash(Encoding.UTF8.GetBytes(validPassword));
            var user = _fixture.Build<UserDao>()
                .With(u => u.PasswordHash, hashedPassword)
                .With(u => u.PasswordSalt, passwordSalt)
                .Create();

            var tokens = _fixture.Create<AuthTokens>();
            var loginRequest = _fixture.Build<LoginRequest>()
                .With(r => r.Email, validEmail)
                .With(r => r.Password, validPassword)
                .Create();

            _validationService.ValidateAsync(loginRequest).Returns(Task.CompletedTask);
            _userRepository.GetByEmailAsync(validEmail).Returns(user);
            _tokenService.GenerateTokensAsync(user).Returns(tokens);

            // Act
            var result = await _authService.AuthenticateAsync(loginRequest);

            // Assert
            result.Should().BeEquivalentTo(tokens);

            // Verify interactions
            await _validationService.Received(1).ValidateAsync(loginRequest);
            await _userRepository.Received(1).GetByEmailAsync(validEmail);
            await _tokenService.Received(1).GenerateTokensAsync(user);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowUnauthorizedAccessException_WhenEmailIsInvalid()
        {
            // Arrange
            var invalidEmail = "wrong@example.com";
            var loginRequest = _fixture.Build<LoginRequest>()
                .With(r => r.Email, invalidEmail)
                .Create();

            _validationService.ValidateAsync(loginRequest).Returns(Task.CompletedTask);
            _userRepository.GetByEmailAsync(invalidEmail).Returns((UserDao)null);

            // Act
            Func<Task> act = async () => await _authService.AuthenticateAsync(loginRequest);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("Invalid login or password");

            // Verify interactions
            await _validationService.Received(1).ValidateAsync(loginRequest);
            await _userRepository.Received(1).GetByEmailAsync(invalidEmail);
            await _tokenService.DidNotReceive().GenerateTokensAsync(Arg.Any<UserDao>());
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowException_WhenValidationFails()
        {
            // Arrange
            var invalidRequest = _fixture.Create<LoginRequest>();

            // Mock validation service to throw an exception
            _validationService.ValidateAsync(invalidRequest).Throws(new ValidationException("Validation failed"));

            // Act
            Func<Task> act = async () => await _authService.AuthenticateAsync(invalidRequest);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("Validation failed");

            // Verify interactions
            await _validationService.Received(1).ValidateAsync(invalidRequest);
            await _userRepository.DidNotReceive().GetByEmailAsync(Arg.Any<string>());
            await _tokenService.DidNotReceive().GenerateTokensAsync(Arg.Any<UserDao>());
        }
    }
}

using Application.Services.Interfaces;
using Application.Services;
using AutoMapper;
using Moq;
using Application.Models;
using DataAccessLayer.Models;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using FluentAssertions;
using DataAccessLayer.Repositories.Interfaces;

namespace Application.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _tokenServiceMock = new Mock<ITokenService>();
            _validationServiceMock = new Mock<IValidationService>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _authService = new AuthService(_userRepositoryMock.Object, _tokenServiceMock.Object, _validationServiceMock.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnAuthTokens_WhenLoginIsValid()
        {
            // Arrange
            var validEmail = "test@example.com";
            var validPassword = "password123";

            var hashedPassword = new HMACSHA512(Encoding.UTF8.GetBytes("salt")).ComputeHash(Encoding.UTF8.GetBytes(validPassword));

            var user = new User
            {
                Login = validEmail,
                PasswordSalt = Encoding.UTF8.GetBytes("salt"),
                PasswordHash = hashedPassword
            };

            var tokens = new AuthTokens
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token"
            };

            _validationServiceMock
                .Setup(v => v.ValidateAsync(It.IsAny<LoginRequest>()))
                .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(u => u.GetByEmailAsync(validEmail))
                .ReturnsAsync(user);

            _tokenServiceMock
                .Setup(t => t.GenerateTokensAsync(user))
                .ReturnsAsync(tokens);

            var request = new LoginRequest { Email = validEmail, Password = validPassword };

            // Act
            var result = await _authService.AuthenticateAsync(request);

            // Assert
            result.Should().BeEquivalentTo(tokens);

            // Verify interactions
            _validationServiceMock.Verify(v => v.ValidateAsync(request), Times.Once);
            _userRepositoryMock.Verify(u => u.GetByEmailAsync(validEmail), Times.Once);
            _tokenServiceMock.Verify(t => t.GenerateTokensAsync(user), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowUnauthorizedAccessException_WhenEmailIsInvalid()
        {
            // Arrange
            var invalidEmail = "wrong@example.com";

            _validationServiceMock
                .Setup(v => v.ValidateAsync(It.IsAny<LoginRequest>()))
                .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(u => u.GetByEmailAsync(invalidEmail))
                .ReturnsAsync((User)null);

            var request = new LoginRequest { Email = invalidEmail, Password = "password123" };

            // Act
            Func<Task> act = async () => await _authService.AuthenticateAsync(request);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid login or password");

            // Verify interactions
            _validationServiceMock.Verify(v => v.ValidateAsync(request), Times.Once);
            _userRepositoryMock.Verify(u => u.GetByEmailAsync(invalidEmail), Times.Once);
            _tokenServiceMock.Verify(t => t.GenerateTokensAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowUnauthorizedAccessException_WhenPasswordIsInvalid()
        {
            // Arrange
            var validEmail = "test@example.com";
            var invalidPassword = "wrongpassword";

            var correctPasswordHash = new HMACSHA512(Encoding.UTF8.GetBytes("salt")).ComputeHash(Encoding.UTF8.GetBytes("password123"));

            var user = new User
            {
                Login = validEmail,
                PasswordSalt = Encoding.UTF8.GetBytes("salt"),
                PasswordHash = correctPasswordHash
            };

            _validationServiceMock
                .Setup(v => v.ValidateAsync(It.IsAny<LoginRequest>()))
                .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(u => u.GetByEmailAsync(validEmail))
                .ReturnsAsync(user);

            var request = new LoginRequest { Email = validEmail, Password = invalidPassword };

            // Act
            Func<Task> act = async () => await _authService.AuthenticateAsync(request);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid login or password");

            // Verify interactions
            _validationServiceMock.Verify(v => v.ValidateAsync(request), Times.Once);
            _userRepositoryMock.Verify(u => u.GetByEmailAsync(validEmail), Times.Once);
            _tokenServiceMock.Verify(t => t.GenerateTokensAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowException_WhenValidationFails()
        {
            // Arrange
            var invalidRequest = new LoginRequest { Email = "", Password = "" };

            // Mock validation service to throw an exception
            _validationServiceMock
                .Setup(v => v.ValidateAsync(invalidRequest))
                .ThrowsAsync(new Exception("Validation failed"));

            // Act
            Func<Task> act = async () => await _authService.AuthenticateAsync(invalidRequest);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Validation failed");

            // Verify interactions
            _validationServiceMock.Verify(v => v.ValidateAsync(invalidRequest), Times.Once);
            _userRepositoryMock.Verify(u => u.GetByEmailAsync(It.IsAny<string>()), Times.Never);
            _tokenServiceMock.Verify(t => t.GenerateTokensAsync(It.IsAny<User>()), Times.Never);
        }
    }
}

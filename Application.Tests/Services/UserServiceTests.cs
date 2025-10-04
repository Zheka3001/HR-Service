using Application.Models;
using Application.Services;
using Application.Services.Interfaces;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using FluentAssertions;
using Moq;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Application.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IWorkGroupRepository> _workGroupRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _validationServiceMock = new Mock<IValidationService>();
            _mapper = new Mock<IMapper>();
            _workGroupRepositoryMock = new Mock<IWorkGroupRepository>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _mapper.Object,
                _validationServiceMock.Object,
                _workGroupRepositoryMock.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ValidUserInfoAndWorkGroup_ShouldCallRepositoryAndSaveChanges()
        {
            // Arrange
            var user = new RegisterUser
            {
                FirstName = "John",
                LastName = "Doe",
                MiddleName = null,
                Login = "john@test.com",
                Password = "Pass123",
                WorkGroupId = 1
            };

            var hmac = new HMACSHA512();
            var userToSaveInDatabase = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Login = user.Login,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password)),
                PasswordSalt = hmac.Key,
                WorkGroupId = user.WorkGroupId
            };

            _mapper
                .Setup(m => m.Map<User>(user))
                .Returns(userToSaveInDatabase);

            _validationServiceMock
                .Setup(v => v.ValidateAsync(user))
                .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(ur => ur.UserExistsAsync(user.Login))
                .ReturnsAsync(false);

            _workGroupRepositoryMock
                .Setup(wg => wg.WorkGroupExistsAsync(user.WorkGroupId))
                .ReturnsAsync(true);

            // Act
            await _userService.RegisterUserAsync(user);

            // Assert
            _validationServiceMock.Verify(s => s.ValidateAsync(user), Times.Once);
            _userRepositoryMock.Verify(ur => ur.UserExistsAsync(user.Login), Times.Once);
            _workGroupRepositoryMock.Verify(wg => wg.WorkGroupExistsAsync(user.WorkGroupId), Times.Once);
            _userRepositoryMock.Verify(ur => ur.AddUserAsync(userToSaveInDatabase), Times.Once);
            _userRepositoryMock.Verify(ur => ur.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_InvalidUserInfo_ShouldThrowException()
        {
            // Arrange
            var invalidUserInfo = new RegisterUser
            {
                FirstName = "",
                LastName = "",
                MiddleName = null,
                Login = "john",
                Password = "Pass",
                WorkGroupId = -1
            };

            _validationServiceMock
                .Setup(v => v.ValidateAsync(invalidUserInfo))
                .ThrowsAsync(new Exception("Validation failed"));

            // Act
            Func<Task> act = async () => await _userService.RegisterUserAsync(invalidUserInfo);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage($"Validation failed");

            _validationServiceMock.Verify(s => s.ValidateAsync(invalidUserInfo), Times.Once);
            _userRepositoryMock.Verify(ur => ur.UserExistsAsync(It.IsAny<string>()), Times.Never);
            _workGroupRepositoryMock.Verify(wg => wg.WorkGroupExistsAsync(It.IsAny<int>()), Times.Never);
            _userRepositoryMock.Verify(ur => ur.AddUserAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(ur => ur.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RegisterUserAsync_ExistedUserLogin_ShouldThrowException()
        {
            // Arrange
            var user = new RegisterUser
            {
                FirstName = "John",
                LastName = "Doe",
                MiddleName = null,
                Login = "john@test.com",
                Password = "Pass123",
                WorkGroupId = 1
            };

            _validationServiceMock
                .Setup(v => v.ValidateAsync(user))
                .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(ur => ur.UserExistsAsync(user.Login))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _userService.RegisterUserAsync(user);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage($"User with login {user.Login} already exists.");

            _validationServiceMock.Verify(s => s.ValidateAsync(user), Times.Once);
            _userRepositoryMock.Verify(ur => ur.UserExistsAsync(user.Login), Times.Once);
            _workGroupRepositoryMock.Verify(wg => wg.WorkGroupExistsAsync(It.IsAny<int>()), Times.Never);
            _userRepositoryMock.Verify(ur => ur.AddUserAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(ur => ur.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RegisterUserAsync_WorkGroupIdIsInvalid_ShouldThrowException()
        {
            // Arrange
            var user = new RegisterUser
            {
                FirstName = "John",
                LastName = "Doe",
                MiddleName = null,
                Login = "john@test.com",
                Password = "Pass123",
                WorkGroupId = 10
            };

            _validationServiceMock
                .Setup(v => v.ValidateAsync(user))
                .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(ur => ur.UserExistsAsync(user.Login))
                .ReturnsAsync(false);

            _workGroupRepositoryMock
                .Setup(wg => wg.WorkGroupExistsAsync(user.WorkGroupId))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _userService.RegisterUserAsync(user);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage($"Work group with id {user.WorkGroupId} doesn't exists");

            _validationServiceMock.Verify(s => s.ValidateAsync(user), Times.Once);
            _userRepositoryMock.Verify(ur => ur.UserExistsAsync(user.Login), Times.Once);
            _workGroupRepositoryMock.Verify(wg => wg.WorkGroupExistsAsync(user.WorkGroupId), Times.Once);
            _userRepositoryMock.Verify(ur => ur.AddUserAsync(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(ur => ur.SaveChangesAsync(), Times.Never);
        }
    }
}

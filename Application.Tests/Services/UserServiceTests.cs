using Application.Exceptions;
using Application.Models;
using Application.Services;
using Application.Services.Interfaces;
using Application.Tests.Extenstions;
using AutoFixture;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Application.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Fixture _fixture;
        private readonly IUserRepository _userRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly IWorkGroupRepository _workGroupRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _fixture = new Fixture().FixCircularReference();
            _userRepository = Substitute.For<IUserRepository>();
            _validationService = Substitute.For<IValidationService>();
            _mapper = Substitute.For<IMapper>();
            _workGroupRepository = Substitute.For<IWorkGroupRepository>();

            _userService = new UserService(_userRepository, _mapper, _validationService, _workGroupRepository);
        }

        [Fact]
        public async Task RegisterUserAsync_ValidUserInfoAndWorkGroup_ShouldCallRepositoryAndSaveChanges()
        {
            // Arrange
            var user = _fixture.Create<RegisterUser>();
            var userToSaveInDatabase = _fixture.Create<UserDao>();

            _mapper.Map<UserDao>(user).Returns(userToSaveInDatabase);
            _validationService.ValidateAsync(user).Returns(Task.CompletedTask);
            _userRepository.UserExistsAsync(user.Login).Returns(false);
            _workGroupRepository.WorkGroupExistsAsync(user.WorkGroupId).Returns(true);

            // Act
            await _userService.RegisterUserAsync(user);

            // Assert
            await _validationService.Received(1).ValidateAsync(user);
            await _userRepository.Received(1).UserExistsAsync(user.Login);
            await _workGroupRepository.Received(1).WorkGroupExistsAsync(user.WorkGroupId);
            await _userRepository.Received(1).AddUserAsync(userToSaveInDatabase);
            await _userRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task RegisterUserAsync_InvalidUserInfo_ShouldThrowException()
        {
            // Arrange
            var invalidUserInfo = _fixture.Create<RegisterUser>();

            _validationService.ValidateAsync(invalidUserInfo).Throws(new ValidationException("Validation failed"));

            // Act
            Func<Task> act = async () => await _userService.RegisterUserAsync(invalidUserInfo);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage($"Validation failed");

            await _validationService.Received(1).ValidateAsync(invalidUserInfo);
            await _userRepository.DidNotReceive().UserExistsAsync(Arg.Any<string>());
            await _workGroupRepository.DidNotReceive().WorkGroupExistsAsync(Arg.Any<int>());
            await _userRepository.DidNotReceive().AddUserAsync(Arg.Any<UserDao>());
            await _userRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task RegisterUserAsync_ExistedUserLogin_ShouldThrowException()
        {
            // Arrange
            var user = _fixture.Create<RegisterUser>();

            _validationService.ValidateAsync(user).Returns(Task.CompletedTask);
            _userRepository.UserExistsAsync(user.Login).Returns(true);

            // Act
            Func<Task> act = async () => await _userService.RegisterUserAsync(user);

            // Assert
            await act.Should()
                .ThrowAsync<BadArgumentException>()
                .WithMessage($"User with login {user.Login} already exists.");

            await _validationService.Received(1).ValidateAsync(user);
            await _userRepository.Received(1).UserExistsAsync(user.Login);
            await _workGroupRepository.DidNotReceive().WorkGroupExistsAsync(Arg.Any<int>());
            await _userRepository.DidNotReceive().AddUserAsync(Arg.Any<UserDao>());
            await _userRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task RegisterUserAsync_WorkGroupIdIsInvalid_ShouldThrowException()
        {
            // Arrange
            var user = _fixture.Create<RegisterUser>();

            _validationService.ValidateAsync(user).Returns(Task.CompletedTask);
            _userRepository.UserExistsAsync(user.Login).Returns(false);
            _workGroupRepository.WorkGroupExistsAsync(user.WorkGroupId).Returns(false);

            // Act
            Func<Task> act = async () => await _userService.RegisterUserAsync(user);

            // Assert
            await act.Should()
                .ThrowAsync<BadArgumentException>()
                .WithMessage($"Work group with id {user.WorkGroupId} doesn't exists");

            await _validationService.Received(1).ValidateAsync(user);
            await _userRepository.Received(1).UserExistsAsync(user.Login);
            await _workGroupRepository.Received(1).WorkGroupExistsAsync(user.WorkGroupId);
            await _userRepository.DidNotReceive().AddUserAsync(Arg.Any<UserDao>());
            await _userRepository.DidNotReceive().SaveChangesAsync();
        }
    }
}

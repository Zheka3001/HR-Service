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
using Shouldly;
using Xunit;

namespace Application.Tests.Services
{
    public class ApplicantServiceTests
    {
        private readonly Fixture _fixture;
        private readonly IApplicantRepository _applicantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IValidationService _validator;
        private readonly IMapper _mapper;
        private readonly ApplicantService _applicantService;

        public ApplicantServiceTests()
        {
            _fixture = new Fixture().FixCircularReference();
            _applicantRepository = Substitute.For<IApplicantRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _validator = Substitute.For<IValidationService>();
            _mapper = Substitute.For<IMapper>();

            _applicantService = new ApplicantService(
                _validator,
                _mapper,
                _applicantRepository,
                _userRepository
            );
        }

        [Fact]
        public async Task CreateApplicantAsync_ShouldCreateApplicant_WhenDataIsValid()
        {
            // Arrange
            var userId = 1;
            var workGroupId = 1;
            var createdApplicantId = 1;
            var createApplicant = _fixture.Create<CreateApplicantRequest>();
            var applicantDao = _fixture.Build<ApplicantDao>()
                .With(a => a.Id, createdApplicantId)
                .With(a => a.WorkGroupId, workGroupId)
                .With(a => a.CreatedById, userId)
                .Create();

            var userDao = _fixture.Build<UserDao>()
                .With(u => u.Id, userId)
                .With(u => u.WorkGroupId, workGroupId)
                .Create();

            _validator.ValidateAsync(createApplicant).Returns(Task.CompletedTask);
            _userRepository.GetByIdAsync(userId).Returns(userDao);
            _mapper.Map<ApplicantDao>(createApplicant).Returns(applicantDao);
            _applicantRepository.InsertAsync(applicantDao).Returns(Task.CompletedTask);
            _applicantRepository.SaveChangesAsync().Returns(Task.CompletedTask);

            // Act
            var applicantId = await _applicantService.CreateApplicantAsync(createApplicant, userId);

            // Assert
            applicantId.ShouldBe(createdApplicantId);

            await _validator.Received(1).ValidateAsync(createApplicant);
            await _userRepository.Received(1).GetByIdAsync(userId);
            await _applicantRepository.Received(1).InsertAsync(applicantDao);
            await _applicantRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CreateApplicantAsync_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var userId = 1;
            var invalidCreateApplicant = _fixture.Create<CreateApplicantRequest>();

            _validator.ValidateAsync(invalidCreateApplicant).Throws(new ValidationException("Validation failed"));

            // Act
            Func<Task> act = async () => await _applicantService.CreateApplicantAsync(invalidCreateApplicant, userId);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage($"Validation failed"); ;

            // Verify validator called
            await _validator.Received(1).ValidateAsync(invalidCreateApplicant);

            // Ensure repository methods and mapper are not called
            await _userRepository.DidNotReceive().GetByIdAsync(userId);
            _mapper.DidNotReceive().Map<ApplicantDao>(Arg.Any<CreateApplicantRequest>());
            await _applicantRepository.DidNotReceive().InsertAsync(Arg.Any<ApplicantDao>());
            await _applicantRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task CreateApplicantAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotFound()
        {
            // Arrange
            var userId = 999; // Invalid userId
            var createApplicant = _fixture.Create<CreateApplicantRequest>();

            _validator.ValidateAsync(createApplicant).Returns(Task.CompletedTask);

            // Mock user repository to return null for invalid UserId
            _userRepository.GetByIdAsync(userId).Returns((UserDao)null);

            // Act
            Func<Task> act = async () => await _applicantService.CreateApplicantAsync(createApplicant, userId);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("User is not authorized or does not exist.");

            // Verify interactions
            await _validator.Received(1).ValidateAsync(createApplicant);
            await _userRepository.Received(1).GetByIdAsync(userId);    // Ensure mapper and repository methods are not called
            _mapper.DidNotReceive().Map<ApplicantDao>(Arg.Any<CreateApplicantRequest>());
            await _applicantRepository.DidNotReceive().InsertAsync(Arg.Any<ApplicantDao>());
            await _applicantRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task CreateApplicantAsync_ShouldThrowInternalErrorException_WhenWorkGroupIsNull()
        {
            // Arrange
            var userId = 1;
            var createApplicant = _fixture.Create<CreateApplicantRequest>();
            var userDao = _fixture.Build<UserDao>()
                .With(u => u.WorkGroupId, (int?)null)
                .Create();

            _validator.ValidateAsync(createApplicant).Returns(Task.CompletedTask);

            // Mock user repository to return null for invalid UserId
            _userRepository.GetByIdAsync(userId).Returns(userDao);

            // Act
            Func<Task> act = async () => await _applicantService.CreateApplicantAsync(createApplicant, userId);

            // Assert
            await act.Should().ThrowAsync<InternalErrorException>()
                .WithMessage("Work group cannot be null for hrs");

            // Verify interactions
            await _validator.Received(1).ValidateAsync(createApplicant);
            await _userRepository.Received(1).GetByIdAsync(userId);
            _mapper.DidNotReceive().Map<ApplicantDao>(Arg.Any<CreateApplicantRequest>());
            await _applicantRepository.DidNotReceive().InsertAsync(Arg.Any<ApplicantDao>());
            await _applicantRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateApplicantAsync_ShouldUpdateApplicant_WhenDataIsValid()
        {
            // Arrange
            var userId = 1;
            var workGroupId = 1;
            var updateApplicantRequest = _fixture.Create<UpdateApplicantRequest>();

            var user = _fixture.Build<UserDao>()
                .With(u => u.Id, userId)
                .With(u => u.WorkGroupId, workGroupId)
                .Create();
            
            var applicantDao = _fixture.Build<ApplicantDao>()
                .With(a => a.CreatedById, userId)
                .With(a => a.WorkGroupId, workGroupId)
                .Create();

            var newSocialNetworksDao = _fixture.Create<ICollection<SocialNetworkDao>>();

            _validator.ValidateAsync(updateApplicantRequest).Returns(Task.CompletedTask);
            _userRepository.GetByIdAsync(userId).Returns(user);
            _applicantRepository.GetByIdAsync(updateApplicantRequest.Id).Returns(applicantDao);
            _mapper.Map<ICollection<SocialNetworkDao>>(updateApplicantRequest.SocialNetworks).Returns(newSocialNetworksDao);
            _applicantRepository.SaveChangesAsync().Returns(Task.CompletedTask);

            // Act
            await _applicantService.UpdateApplicantAsync(updateApplicantRequest, userId);

            // Assert
            applicantDao.ApplicantInfo.FirstName.ShouldBe(updateApplicantRequest.FirstName);
            applicantDao.ApplicantInfo.LastName.ShouldBe(updateApplicantRequest.LastName);
            applicantDao.ApplicantInfo.MiddleName.ShouldBe(updateApplicantRequest.MiddleName);
            applicantDao.ApplicantInfo.Email.ShouldBe(updateApplicantRequest.Email);
            applicantDao.ApplicantInfo.PhoneNumber.ShouldBe(updateApplicantRequest.PhoneNumber);
            applicantDao.ApplicantInfo.Country.ShouldBe(updateApplicantRequest.Country);
            applicantDao.ApplicantInfo.DateOfBirth.ShouldBe(updateApplicantRequest.DateOfBirth);
            applicantDao.WorkSchedule.ShouldBe((WorkScheduleDao)updateApplicantRequest.WorkSchedule);
            applicantDao.ApplicantInfo.SocialNetworks.ShouldBe(newSocialNetworksDao);

            // Verify method interactions
            await _validator.Received(1).ValidateAsync(updateApplicantRequest);
            await _userRepository.Received(1).GetByIdAsync(userId);
            _mapper.Received(1).Map<ICollection<SocialNetworkDao>>(updateApplicantRequest.SocialNetworks);
            await _applicantRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateApplicantAsync_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var userId = 1;
            var invalidUpdateUpplicantRequest = _fixture.Create<UpdateApplicantRequest>();

            _validator.ValidateAsync(invalidUpdateUpplicantRequest).Throws(new ValidationException("Validation failed"));

            // Act
            Func<Task> act = async () => await _applicantService.UpdateApplicantAsync(invalidUpdateUpplicantRequest, userId);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage($"Validation failed");

            // Verify validator called
            await _validator.Received(1).ValidateAsync(invalidUpdateUpplicantRequest);

            // Ensure repository methods and mapper are not called
            await _userRepository.DidNotReceive().GetByIdAsync(userId);
            await _applicantRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateApplicantAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotFound()
        {
            // Arrange
            var userId = 999; // Invalid userId
            var updateApplicant = _fixture.Create<UpdateApplicantRequest>();

            _validator.ValidateAsync(updateApplicant).Returns(Task.CompletedTask);

            // Mock user repository to return null for invalid UserId
            _userRepository.GetByIdAsync(userId).Returns((UserDao)null);

            // Act
            Func<Task> act = async () => await _applicantService.UpdateApplicantAsync(updateApplicant, userId);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("User is not authorized or does not exist.");

            // Verify interactions
            await _validator.Received(1).ValidateAsync(updateApplicant);
            await _userRepository.Received(1).GetByIdAsync(userId);
            await _applicantRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateApplicantAsync_ShouldThrowInternalErrorException_WhenWorkGroupIsNull()
        {
            // Arrange
            var userId = 1;
            var updateApplicant = _fixture.Create<UpdateApplicantRequest>();
            var userDao = _fixture.Build<UserDao>()
                .With(u => u.WorkGroupId, (int?)null)
                .Create();

            _validator.ValidateAsync(updateApplicant).Returns(Task.CompletedTask);

            // Mock user repository to return null for invalid UserId
            _userRepository.GetByIdAsync(userId).Returns(userDao);

            // Act
            Func<Task> act = async () => await _applicantService.UpdateApplicantAsync(updateApplicant, userId);

            // Assert
            await act.Should().ThrowAsync<InternalErrorException>()
                .WithMessage("Work group cannot be null for hrs");

            // Verify interactions
            await _validator.Received(1).ValidateAsync(updateApplicant);
            await _userRepository.Received(1).GetByIdAsync(userId);
            await _applicantRepository.DidNotReceive().SaveChangesAsync();
        }
    }
}

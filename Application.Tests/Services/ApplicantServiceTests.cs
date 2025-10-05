using Application.Models;
using Application.Services;
using Application.Services.Interfaces;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;
using FluentAssertions;
using FluentValidation;
using Moq;
using Xunit;

namespace Application.Tests.Services
{
    public class ApplicantServiceTests
    {
        private readonly Mock<IApplicantRepository> _applicantRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IValidationService> _validatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ApplicantService _applicantService;

        public ApplicantServiceTests()
        {
            _applicantRepositoryMock = new Mock<IApplicantRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _validatorMock = new Mock<IValidationService>();
            _mapperMock = new Mock<IMapper>();

            _applicantService = new ApplicantService(
                _validatorMock.Object,
                _mapperMock.Object,
                _applicantRepositoryMock.Object,
                _userRepositoryMock.Object
            );
        }

        [Fact]
        public async Task CreateApplicantAsync_ShouldCreateApplicant_WhenDataIsValid()
        {
            // Arrange
            var userId = 1;
            var workGroupId = 1;
            var createApplicant = new CreateApplicantRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Country = "US",
                DateOfBirth = new DateTime(1990, 1, 1),
                CreateSocialNetworkInfoRequests = new List<CreateSocialNetworkInfoRequest>
                {
                    new CreateSocialNetworkInfoRequest
                    {
                        UserName = "johnDoeLinkedIn",
                        Type = SocialNetworkType.LinkedIn
                    }
                }
            };

            var user = new User { Id = userId, Login = "john.doe@example.com", WorkGroupId = workGroupId };
            var mappedApplicant = new Applicant
            {
                ApplicantInfo = new ApplicantInfo
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com"
                },
                CreatedById = userId,
                WorkGroupId = workGroupId
            };

            var createdApplicantResponse = new CreateApplicantResponse
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                WorkGroupId = workGroupId
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(createApplicant))
                .Returns(Task.CompletedTask);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<Applicant>(createApplicant))
                .Returns(mappedApplicant);

            _mapperMock.Setup(m => m.Map<CreateApplicantResponse>(mappedApplicant))
                .Returns(createdApplicantResponse);

            _applicantRepositoryMock.Setup(r => r.InsertAsync(mappedApplicant))
                .Returns(Task.CompletedTask);

            _applicantRepositoryMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var createdApplicant = await _applicantService.CreateApplicantAsync(createApplicant, userId);

            // Assert
            createdApplicant.Should().NotBeNull();

            // Verify method interactions
            _validatorMock.Verify(v => v.ValidateAsync(createApplicant), Times.Once);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _mapperMock.Verify(m => m.Map<Applicant>(createApplicant), Times.Once);
            _applicantRepositoryMock.Verify(r => r.InsertAsync(mappedApplicant), Times.Once);
            _applicantRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateApplicantAsync_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var userId = 1;
            var invalidCreateApplicant = new CreateApplicantRequest
            {
                FirstName = "", // Invalid: Empty FirstName
                LastName = "Doe",
                Email = "not.an.email", // Invalid: Incorrect email format
                PhoneNumber = "12345", // Invalid: Weak phone number
                Country = "XYZ",
                DateOfBirth = new DateTime(3000, 1, 1) // Invalid: Future date
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(invalidCreateApplicant))
                .ThrowsAsync(new Exception("Validation failed"));

            // Act
            Func<Task> act = async () => await _applicantService.CreateApplicantAsync(invalidCreateApplicant, userId);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage($"Validation failed"); ;

            // Verify validator called
            _validatorMock.Verify(v => v.ValidateAsync(invalidCreateApplicant), Times.Once);

            // Ensure repository methods and mapper are not called
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Never);
            _mapperMock.Verify(m => m.Map<Applicant>(It.IsAny<CreateApplicantRequest>()), Times.Never);
            _applicantRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Applicant>()), Times.Never);
            _applicantRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateApplicantAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotFound()
        {
            // Arrange
            var userId = 999; // Invalid userId
            var createApplicant = new CreateApplicantRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Country = "US",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(createApplicant))
                .Returns(Task.CompletedTask);

            // Mock user repository to return null for invalid UserId
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _applicantService.CreateApplicantAsync(createApplicant, userId);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("User is not authorized or does not exist.");

            // Verify interactions
            _validatorMock.Verify(v => v.ValidateAsync(createApplicant), Times.Once);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);

            // Ensure mapper and repository methods are not called
            _mapperMock.Verify(m => m.Map<Applicant>(It.IsAny<CreateApplicantRequest>()), Times.Never);
            _applicantRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Applicant>()), Times.Never);
            _applicantRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}

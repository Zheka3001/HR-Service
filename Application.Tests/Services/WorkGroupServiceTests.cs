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
using Microsoft.EntityFrameworkCore.Storage;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Xunit;

namespace Application.Tests.Services
{
    public class WorkGroupServiceTests
    {
        private readonly Fixture _fixture;
        private readonly IValidationService _validationService;
        private readonly IWorkGroupRepository _workGroupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly WorkGroupService _workGroupService;

        public WorkGroupServiceTests()
        {
            _fixture = new Fixture().FixCircularReference();
            _validationService = Substitute.For<IValidationService>();
            _workGroupRepository = Substitute.For<IWorkGroupRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = Substitute.For<IMapper>();

            _workGroupService = new WorkGroupService(_workGroupRepository, _validationService, _mapper, _userRepository);
        }

        [Fact]
        public async Task AddAsync_ShouldCallRepositorySaveChangesAndReturnWorkGroupId_ForValidWorkGroupRequest()
        {
            // Arrange
            var createdWorkGroupId = 1;
            var workGroupCreateRequest = _fixture.Create<CreateWorkGroup>();
            var workGroupDao = _fixture.Build<WorkGroupDao>()
                .With(wg => wg.Id, createdWorkGroupId)
                .Create();

            _validationService.ValidateAsync(workGroupCreateRequest).Returns(Task.CompletedTask);
            _mapper.Map<WorkGroupDao>(workGroupCreateRequest).Returns(workGroupDao);
            _workGroupRepository.InsertAsync(workGroupDao).Returns(Task.CompletedTask);

            // Act
            var workGroupId = await _workGroupService.AddAsync(workGroupCreateRequest);

            //Assert
            workGroupId.ShouldBe(createdWorkGroupId);

            await _validationService.Received(1).ValidateAsync(workGroupCreateRequest);
            _mapper.Received(1).Map<WorkGroupDao>(workGroupCreateRequest);
            await _workGroupRepository.Received(1).InsertAsync(workGroupDao);
            await _workGroupRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task AddAsync_ShouldThrowValidationException_ForInvalidRequest()
        {
            // Arrange
            var workGroupCreateRequest = _fixture.Create<CreateWorkGroup>();

            _validationService.ValidateAsync(workGroupCreateRequest).Throws(new ValidationException("Validation failed"));

            // Act
            Func<Task> act = async () => await _workGroupService.AddAsync(workGroupCreateRequest);

            //Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage($"Validation failed");

            await _validationService.Received(1).ValidateAsync(workGroupCreateRequest);
            _mapper.DidNotReceive().Map<WorkGroupDao>(Arg.Any<CreateWorkGroup>());
            await _workGroupRepository.DidNotReceive().InsertAsync(Arg.Any<WorkGroupDao>());
            await _workGroupRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task MoveHrsAsync_ShouldUpdateWorkGroupId_ForHrsAndApplicants()
        {
            // Arrange
            var validWorkGroupId = 100;
            var validUserIds = new List<int>() { 1, 2 };

            var hrUsers = new List<UserDao>
            {
                new UserDao { Id = 1, WorkGroupId = 10, CreatedApplicants = new List<ApplicantDao>
                {
                    new ApplicantDao { Id = 101, WorkGroupId = 10 },
                    new ApplicantDao { Id = 102, WorkGroupId = 10 }
                }},
                new UserDao { Id = 2, WorkGroupId = 10, CreatedApplicants = new List<ApplicantDao>
                {
                    new ApplicantDao { Id = 103, WorkGroupId = 10 }
                }},
            };

            var request = _fixture.Build<MoveHrsRequest>()
                .With(r => r.WorkGroupId, validWorkGroupId)
                .With(r => r.UserIds, validUserIds)
                .Create();

            // Mock validation
            _validationService.ValidateAsync(Arg.Any<MoveHrsRequest>()).Returns(Task.CompletedTask);
            _workGroupRepository.WorkGroupExistsAsync(validWorkGroupId).Returns(true);
            _userRepository.GetByIdsAsync(validUserIds).Returns(hrUsers);

            // Mock transaction
            var transactionMock = Substitute.For<IDbContextTransaction>();
            _workGroupRepository.BeginTransaction().Returns(transactionMock);

            // Act
            await _workGroupService.MoveHrsAsync(request);

            //Assert
            hrUsers.Should().OnlyContain(user => user.WorkGroupId == validWorkGroupId);

            hrUsers.SelectMany(u => u.CreatedApplicants)
                .Should().OnlyContain(a => a.WorkGroupId == validWorkGroupId);

            await _userRepository.Received(1).SaveChangesAsync();
            await transactionMock.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task MoveHrsAsync_ShouldThrowArgumentException_WhenWorkGroupDoesNotExist()
        {
            // Arrange
            var invalidWorkGroupId = 999;

            var request = _fixture.Build<MoveHrsRequest>()
                .With(r => r.WorkGroupId, invalidWorkGroupId)
                .With(r => r.UserIds, [1])
                .Create();

            _workGroupRepository.WorkGroupExistsAsync(invalidWorkGroupId).Returns(false);

            // Act
            var act = async () => await _workGroupService.MoveHrsAsync(request);

            // Assert
            await act.Should().ThrowAsync<BadArgumentException>()
                .WithMessage("Destination work group does not exists");

            // Verify transaction is not invoked
            _workGroupRepository.DidNotReceive().BeginTransaction();
        }

        [Fact]
        public async Task MoveHrsAsync_ShouldThrowArgumentException_WhenUserIdsAreInvalid()
        {
            // Arrange
            var validWorkGroupId = 100;
            var requestUserIds = new List<int> { 1, 2, 3 };
            var foundUsers = new List<UserDao>
            {
                new UserDao { Id = 1 },
                new UserDao { Id = 2 }
            };

            _workGroupRepository.WorkGroupExistsAsync(validWorkGroupId).Returns(true);
            _userRepository.GetByIdsAsync(requestUserIds).Returns(foundUsers);

            var request = _fixture.Build<MoveHrsRequest>()
                .With(r => r.WorkGroupId, validWorkGroupId)
                .With(r => r.UserIds, requestUserIds)
                .Create();

            // Act
            Func<Task> act = async () => await _workGroupService.MoveHrsAsync(request);

            // Assert
            await act.Should().ThrowAsync<BadArgumentException>()
                .WithMessage("Invalid HR user IDs provided 3");

            // Verify SaveChangesAsync is not called
            await _userRepository.DidNotReceive().SaveChangesAsync();

            // Verify transaction is not invoked
            _workGroupRepository.DidNotReceive().BeginTransaction();
        }

        [Fact]
        public async Task MoveHrsAsync_ShouldRollbackTransaction_OnFailure()
        {
            // Arrange
            var validWorkGroupId = 100;
            var validUserIds = new List<int> { 1 };

            var hrUsers = new List<UserDao>
            {
                new UserDao { Id = 1, CreatedApplicants = new List<ApplicantDao>() }
            };

            var request = _fixture.Build<MoveHrsRequest>()
                .With(r => r.WorkGroupId, validWorkGroupId)
                .With(r => r.UserIds, validUserIds)
                .Create();

            _workGroupRepository.WorkGroupExistsAsync(validWorkGroupId).Returns(true);
            _userRepository.GetByIdsAsync(validUserIds).Returns(hrUsers);

            // Mock transaction
            var transactionMock = Substitute.For<IDbContextTransaction>();
            _workGroupRepository.BeginTransaction().Returns(transactionMock);

            // Simulate failure in saving changes
            _userRepository.SaveChangesAsync().Throws(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _workGroupService.MoveHrsAsync(request);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Database error");

            // Verify the transaction rollbacks
            await transactionMock.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        }
    }
}

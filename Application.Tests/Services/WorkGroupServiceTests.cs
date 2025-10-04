using Application.Models;
using Application.Services;
using Application.Services.Interfaces;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests.Services
{
    public class WorkGroupServiceTests
    {
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly Mock<IWorkGroupRepository> _workGroupRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly WorkGroupService _workGroupService;

        public WorkGroupServiceTests()
        {
            _validationServiceMock = new Mock<IValidationService>();
            _workGroupRepositoryMock = new Mock<IWorkGroupRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();

            _workGroupService = new WorkGroupService(
                _workGroupRepositoryMock.Object,
                _validationServiceMock.Object,
                _mapperMock.Object,
                _userRepositoryMock.Object
                );
        }

        [Fact]
        public async Task MoveHrsAsync_ShouldUpdateWorkGroupId_ForHrsAndApplicants()
        {
            // Arrange
            var validWorkGroupId = 100;
            var validUserIds = new List<int>() { 1, 2 };

            var hrUsers = new List<User>
            {
                new User { Id = 1, WorkGroupId = 10, CreatedApplicants = new List<Applicant>
                {
                    new Applicant { Id = 101, WorkGroupId = 10 },
                    new Applicant { Id = 102, WorkGroupId = 10 }
                }},
                new User { Id = 2, WorkGroupId = 10, CreatedApplicants = new List<Applicant>
                {
                    new Applicant { Id = 103, WorkGroupId = 10 }
                }},
            };

            // Mock validation
            _validationServiceMock
                .Setup(v => v.ValidateAsync(It.IsAny<MoveHrsRequest>()))
                .Returns(Task.CompletedTask);

            // Mock Work Group existence
            _workGroupRepositoryMock
                .Setup(wr => wr.WorkGroupExistsAsync(validWorkGroupId))
                .ReturnsAsync(true);

            // Mock fetching users
            _userRepositoryMock
                .Setup(ur => ur.GetByIdsAsync(validUserIds))
                .ReturnsAsync(hrUsers);

            // Mock transaction
            var transactionMock = new Mock<IDbContextTransaction>();
            _workGroupRepositoryMock
                .Setup(wr => wr.BeginTransaction())
                .Returns(transactionMock.Object);

            // Act
            var request = new MoveHrsRequest {  WorkGroupId = validWorkGroupId, UserIds = validUserIds };
            await _workGroupService.MoveHrsAsync(request);

            //Assert
            hrUsers.Should().OnlyContain(user => user.WorkGroupId == validWorkGroupId);

            hrUsers.SelectMany(u => u.CreatedApplicants)
                .Should().OnlyContain(a => a.WorkGroupId == validWorkGroupId);

            _userRepositoryMock.Verify(ur => ur.SaveChangesAsync(), Times.Once());

            transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task MoveHrsAsync_ShouldThrowArgumentException_WhenWorkGroupDoesNotExist()
        {
            // Arrange
            var invalidWorkGroupId = 999;

            // Mock WorkGroup existence
            _workGroupRepositoryMock
                .Setup(wr => wr.WorkGroupExistsAsync(invalidWorkGroupId))
                .ReturnsAsync(false);

            // Act
            var request = new MoveHrsRequest { WorkGroupId = invalidWorkGroupId, UserIds = new List<int> { 1 } };
            var act = async () => await _workGroupService.MoveHrsAsync(request);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Destination work group does not exists");

            // Verify transaction is not invoked
            _workGroupRepositoryMock.Verify(wr => wr.BeginTransaction(), Times.Never);
        }

        [Fact]
        public async Task MoveHrsAsync_ShouldThrowArgumentException_WhenUserIdsAreInvalid()
        {
            // Arrange
            var validWorkGroupId = 100;
            var requestUserIds = new List<int> { 1, 2, 3 };
            var foundUsers = new List<User>
            {
                new User { Id = 1 },
                new User { Id = 2 }
            };

            // Mock WorkGroup existence
            _workGroupRepositoryMock
                .Setup(wr => wr.WorkGroupExistsAsync(validWorkGroupId))
                .ReturnsAsync(true);

            // Mock fetching users
            _userRepositoryMock
                .Setup(ur => ur.GetByIdsAsync(requestUserIds))
                .ReturnsAsync(foundUsers);

            // Act
            var request = new MoveHrsRequest { WorkGroupId = validWorkGroupId, UserIds = requestUserIds };
            Func<Task> act = async () => await _workGroupService.MoveHrsAsync(request);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid HR user IDs provided 3");

            // Verify SaveChangesAsync is not called
            _userRepositoryMock.Verify(ur => ur.SaveChangesAsync(), Times.Never);

            // Verify transaction is not invoked
            _workGroupRepositoryMock.Verify(wr => wr.BeginTransaction(), Times.Never);
        }

        [Fact]
        public async Task MoveHrsAsync_ShouldRollbackTransaction_OnFailure()
        {
            // Arrange
            var validWorkGroupId = 100;
            var validUserIds = new List<int> { 1 };

            var hrUsers = new List<User>
            {
                new User { Id = 1, CreatedApplicants = new List<Applicant>() }
            };

            // Mock WorkGroup existence
            _workGroupRepositoryMock
                .Setup(wr => wr.WorkGroupExistsAsync(validWorkGroupId))
                .ReturnsAsync(true);

            // Mock fetching users
            _userRepositoryMock
                .Setup(ur => ur.GetByIdsAsync(validUserIds))
                .ReturnsAsync(hrUsers);

            // Mock transaction
            var transactionMock = new Mock<IDbContextTransaction>();
            _workGroupRepositoryMock
                .Setup(wr => wr.BeginTransaction())
                .Returns(transactionMock.Object);

            // Simulate failure in saving changes
            _userRepositoryMock
                .Setup(ur => ur.SaveChangesAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var request = new MoveHrsRequest { WorkGroupId = validWorkGroupId, UserIds = validUserIds };
            Func<Task> act = async () => await _workGroupService.MoveHrsAsync(request);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Database error");

            // Verify the transaction rollbacks
            transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

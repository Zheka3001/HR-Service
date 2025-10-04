using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace Application.Services
{
    public class WorkGroupService : IWorkGroupService
    {
        private readonly IWorkGroupRepository _workGroupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public WorkGroupService(IWorkGroupRepository workGroupRepository, IValidationService validationService, IMapper mapper, IUserRepository userRepository)
        {
            _workGroupRepository = workGroupRepository;
            _validationService = validationService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task InsertAsync(CreateWorkGroup workGroup)
        {
            await _validationService.ValidateAsync(workGroup);

            await _workGroupRepository.InsertAsync(_mapper.Map<WorkGroup>(workGroup));

            await _workGroupRepository.SaveChangesAsync();
        }

        public async Task MoveHrsAsync(MoveHrsRequest moveHrsRequest)
        {
            await _validationService.ValidateAsync(moveHrsRequest);

            if (!await _workGroupRepository.WorkGroupExistsAsync(moveHrsRequest.WorkGroupId))
            {
                throw new ArgumentException("Destination work group does not exists");
            }

            var hrUsers = await _userRepository.GetByIdsAsync(moveHrsRequest.UserIds);

            var invalidUserIds = moveHrsRequest.UserIds.Except(hrUsers.Select(u => u.Id)).ToList();
            if (invalidUserIds.Any())
            {
                throw new ArgumentException($"Invalid HR user IDs provided {string.Join(", ", invalidUserIds)}");
            }

            using var transaction = _workGroupRepository.BeginTransaction();

            try
            {
                foreach (var user in hrUsers)
                {
                    user.WorkGroupId = moveHrsRequest.WorkGroupId;
                }

                foreach (var applicant in hrUsers.SelectMany(u => u.CreatedApplicants))
                {
                    applicant.WorkGroupId = moveHrsRequest.WorkGroupId;
                }

                await _userRepository.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

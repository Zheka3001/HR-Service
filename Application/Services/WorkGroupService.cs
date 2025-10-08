using Application.Exceptions;
using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;

namespace Application.Services
{
    public class WorkGroupService : IWorkGroupService
    {
        private readonly IWorkGroupRepository _workGroupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly ITransactionProvider _transactionProvider;

        public WorkGroupService(IWorkGroupRepository workGroupRepository, IValidationService validationService, IMapper mapper, IUserRepository userRepository, ITransactionProvider transactionProvider)
        {
            _workGroupRepository = workGroupRepository;
            _validationService = validationService;
            _mapper = mapper;
            _userRepository = userRepository;
            _transactionProvider = transactionProvider;
        }

        public async Task<int> AddAsync(CreateWorkGroup workGroup)
        {
            await _validationService.ValidateAsync(workGroup);

            var workGroupDao = _mapper.Map<WorkGroupDao>(workGroup);

            await _workGroupRepository.InsertAsync(workGroupDao);

            await _workGroupRepository.SaveChangesAsync();

            return workGroupDao.Id;
        }

        public async Task MoveHrsAsync(MoveHrsRequest moveHrsRequest)
        {
            await _validationService.ValidateAsync(moveHrsRequest);

            if (!await _workGroupRepository.WorkGroupExistsAsync(moveHrsRequest.WorkGroupId))
            {
                throw new BadArgumentException("Destination work group does not exists");
            }

            var hrUsers = await _userRepository.GetByIdsAsync(moveHrsRequest.UserIds);

            var invalidUserIds = moveHrsRequest.UserIds.Except(hrUsers.Select(u => u.Id)).ToList();
            if (invalidUserIds.Any())
            {
                throw new BadArgumentException($"Invalid HR user IDs provided {string.Join(", ", invalidUserIds)}");
            }

            using var transaction = await _transactionProvider.BeginTransactionAsync();

            try
            {
                foreach (var user in hrUsers)
                {
                    user.WorkGroupId = moveHrsRequest.WorkGroupId;
                }

                foreach (var applicant in hrUsers.SelectMany(u => u.CreatedApplicants).Where(a => a.CreatedById == a.LastUpdatedById))
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

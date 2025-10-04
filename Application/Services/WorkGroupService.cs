using Application.Models;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace Application.Services
{
    public class WorkGroupService : IWorkGroupService
    {
        private readonly IWorkGroupRepository _workGroupRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public WorkGroupService(IWorkGroupRepository workGroupRepository, IValidationService validationService, IMapper mapper)
        {
            _workGroupRepository = workGroupRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task InsertAsync(CreateWorkGroup workGroup)
        {
            await _validationService.ValidateAsync(workGroup);

            await _workGroupRepository.InsertAsync(_mapper.Map<WorkGroup>(workGroup));

            await _workGroupRepository.SaveChangesAsync();
        }
    }
}

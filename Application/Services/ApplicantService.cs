using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;

namespace Application.Services
{
    public class ApplicantService : IApplicantService
    {
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly IApplicantRepository _applicantRepository;
        private readonly IUserRepository _userRepository;

        public ApplicantService(IValidationService validationService, IMapper mapper, IApplicantRepository applicantRepository, IUserRepository userRepository)
        {
            _validationService = validationService;
            _mapper = mapper;
            _applicantRepository = applicantRepository;
            _userRepository = userRepository;
        }

        public async Task<CreateApplicantResponse> CreateApplicantAsync(CreateApplicantRequest request, int creatorId)
        {
            await _validationService.ValidateAsync(request);

            var user = await _userRepository.GetByIdAsync(creatorId);

            if (user == null || user.WorkGroupId == null)
            {
                throw new UnauthorizedAccessException("User is not authorized or does not exist.");
            }

            request.WorkGroupId = user.WorkGroupId;
            request.CreatorId = creatorId;

            var applicant = _mapper.Map<Applicant>(request);

            await _applicantRepository.InsertAsync(applicant);
            await _applicantRepository.SaveChangesAsync();

            return _mapper.Map<CreateApplicantResponse>(applicant);
        }
    }
}

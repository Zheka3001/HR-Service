using Application.Exceptions;
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

        public async Task<int> CreateApplicantAsync(CreateApplicantRequest request, int creatorId)
        {
            await _validationService.ValidateAsync(request);

            var user = await _userRepository.GetByIdAsync(creatorId);

            if (user == null)
            {
                throw new UnauthorizedException("User is not authorized or does not exist.");
            }

            if (user.WorkGroupId == null)
            {
                throw new InternalErrorException("Work group cannot be null for hrs");
            }

            request.WorkGroupId = user.WorkGroupId;
            request.CreatorId = creatorId;

            var applicant = _mapper.Map<ApplicantDao>(request);

            await _applicantRepository.InsertAsync(applicant);
            await _applicantRepository.SaveChangesAsync();

            return applicant.Id;
        }

        public async Task UpdateApplicantAsync(UpdateApplicantRequest request, int initiatorId)
        {
            await _validationService.ValidateAsync(request);

            var user = await _userRepository.GetByIdAsync(initiatorId);

            if (user == null)
            {
                throw new UnauthorizedException("User is not authorized or does not exist.");
            }

            if (user.WorkGroupId == null)
            {
                throw new InternalErrorException("Work group cannot be null for hrs");
            }

            var applicant = await _applicantRepository.GetByIdAsync(request.Id);

            if (applicant == null)
            {
                throw new BadArgumentException($"Applicant with id {request.Id} doesn't exists");
            }

            if (applicant.WorkGroupId != user.WorkGroupId)
            {
                throw new ForbidException("Applicant belongs to a different work group");
            }

            applicant.ApplicantInfo.FirstName = request.FirstName;
            applicant.ApplicantInfo.LastName = request.LastName;
            applicant.ApplicantInfo.MiddleName = request.MiddleName;
            applicant.ApplicantInfo.Email = request.Email;
            applicant.WorkSchedule = (WorkScheduleDao)request.WorkSchedule;
            applicant.ApplicantInfo.PhoneNumber = request.PhoneNumber;
            applicant.ApplicantInfo.Country = request.Country;
            applicant.ApplicantInfo.DateOfBirth = request.DateOfBirth;
            applicant.ApplicantInfo.SocialNetworks = _mapper.Map<ICollection<SocialNetworkDao>>(request.SocialNetworks);
            applicant.LastUpdateDate = DateTime.UtcNow;

            await _applicantRepository.SaveChangesAsync();
        }
    }
}

using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly IWorkGroupRepository _workGroupRepository;

        public UserService(IUserRepository userRepository, IMapper mapper, IValidationService validationService, IWorkGroupRepository workGroupRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _validationService = validationService;
            _workGroupRepository = workGroupRepository;
        }

        public async Task RegisterUserAsync(RegisterUser user)
        {
            await _validationService.ValidateAsync(user);

            if (await _userRepository.UserExistsAsync(user.Login))
                throw new ArgumentException($"User with login {user.Login} already exists.");

            if (!await _workGroupRepository.WorkGroupExistsAsync(user.WorkGroupId))
            {
                throw new ArgumentException($"Work group with id {user.WorkGroupId} doesn't exists");
            }

            await _userRepository.AddUserAsync(_mapper.Map<User>(user));

            await _userRepository.SaveChangesAsync();
        }
    }
}

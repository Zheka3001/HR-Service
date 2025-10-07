using Application.Exceptions;
using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;

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

        public async Task<int> RegisterUserAsync(RegisterUser user)
        {
            await _validationService.ValidateAsync(user);

            if (await _userRepository.UserExistsAsync(user.Login))
                throw new BadArgumentException($"User with login {user.Login} already exists.");

            if (!await _workGroupRepository.WorkGroupExistsAsync(user.WorkGroupId))
            {
                throw new BadArgumentException($"Work group with id {user.WorkGroupId} doesn't exists");
            }

            var userDao = _mapper.Map<UserDao>(user);

            await _userRepository.AddUserAsync(userDao);

            await _userRepository.SaveChangesAsync();

            return userDao.Id;
        }
    }
}

using Application.Models;
using AutoMapper;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, ITokenService tokenService, IMapper mapper, IValidationService validationService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _mapper = mapper;
            _validationService = validationService;
        }

        public async Task<AuthTokens> AuthenticateAsync(LoginRequest request)
        {
            await _validationService.ValidateAsync(request);

            var user = await _userRepository.GetByEmailAsync(request.Email);

            ValidateAuthorizationLogin(request, user);

            var tokens = await _tokenService.GenerateTokensAsync(user!);

            await _userRepository.UpdateAsync(user!);

            return tokens;
        }

        public async Task RegisterUserAsync(RegisterUser user)
        {
            await _validationService.ValidateAsync(user);

            if (await _userRepository.UserExistsAsync(user.Login))
                throw new ArgumentException($"User with login {user.Login} already exists.");

            await _userRepository.AddUserAsync(_mapper.Map<User>(user));
        }

        private void ValidateAuthorizationLogin(LoginRequest request, User? user)
        {
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid login or password");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) throw new UnauthorizedAccessException("Invalid login or password");
            }

        }
    }
}

using Application.Models;
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

        public UserService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthTokens> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid login or password");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) throw new UnauthorizedAccessException("Invalid login or password");
            }

            var tokens = await _tokenService.GenerateTokensAsync(user);

            await _userRepository.UpdateAsync(user);

            return tokens;
        }

        public async Task RegisterUserAsync(RegisterUser user)
        {
            if (await _userRepository.UserExistsAsync(user.Login))
                throw new ArgumentException($"User with login {user.Login} already exists.");

            using var hmac = new HMACSHA512();

            await _userRepository.AddUserAsync(new User()
            {
                Role = Role.HR,
                Login = user.Login,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password)),
                PasswordSalt = hmac.Key
            });
        }
    }
}

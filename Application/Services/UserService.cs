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
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, ITokenService tokenService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _configuration = configuration;
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

            var tokenRefreshExpireTime = GetRefreshTokenLifeTime();
            var tokens = _tokenService.GenerateTokens(user, tokenRefreshExpireTime);

            user.RefreshToken = tokens.RefreshToken;
            user.RefreshTokenExpiryTime = tokenRefreshExpireTime;
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

        private DateTime GetRefreshTokenLifeTime()
        {
            int expiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]);
            return DateTime.UtcNow.AddDays(expiryDays);
        }
    }
}

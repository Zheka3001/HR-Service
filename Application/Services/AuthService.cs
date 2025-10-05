using Application.Models;
using Application.Services.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IValidationService _validationService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IValidationService validationService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _validationService = validationService;
        }

        public async Task<AuthTokens> AuthenticateAsync(LoginRequest request)
        {
            await _validationService.ValidateAsync(request);

            var user = await _userRepository.GetByEmailAsync(request.Email);

            ValidateAuthorizationLogin(request, user);

            var tokens = await _tokenService.GenerateTokensAsync(user!);

            return tokens;
        }

        public async Task<AuthTokens> RefreshTokensAsync(string accessToken, string refreshToken)
        {
            return await _tokenService.RefreshTokensAsync(accessToken, refreshToken);
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

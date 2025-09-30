using Application.Models;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IUserRepository _userRepository;

        public TokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository)
        {
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            _userRepository = userRepository;
        }

        public async Task<AuthTokens> GenerateTokensAsync(User user)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenLifetime = GetRefreshTokenExpireTime();

            await _refreshTokenRepository.InsertTokenAsync(new RefreshToken
            {
                Token = refreshToken,
                Expires = refreshTokenLifetime,
                UserId = user.Id,
            });

            return new AuthTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = GetAccessTokenExpireTime(),
                RefreshTokenExpiry = refreshTokenLifetime,
            };
        }

        public async Task<AuthTokens> RefreshTokensAsync(string accessToken, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                throw new ArgumentException("Access or refresh token are invalid.");
            }

            var claimUserId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claimUserId) || !int.TryParse(claimUserId, out var userId))
            {
                throw new ArgumentException("Access or refresh token are invalid.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var savedRefreshToken = user.RefreshTokens?.FirstOrDefault(token => token.Token == refreshToken);

            if (savedRefreshToken == null || savedRefreshToken.IsUsed || savedRefreshToken.IsRevoked || savedRefreshToken.Expires < DateTime.UtcNow)
            {
                throw new ArgumentException("Access or refresh token are invalid.");
            }

            savedRefreshToken.IsUsed = true;
            await _refreshTokenRepository.UpdateRefreshToken(savedRefreshToken);

            return await GenerateTokensAsync(user);
        }

        private string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            if (!string.IsNullOrWhiteSpace(user.MiddleName))
            {
                claims.Append(new Claim("MiddleName", user.MiddleName));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]));
            var accessToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: accessTokenExpiration,
                signingCredentials: creds
                );

            return _jwtSecurityTokenHandler.WriteToken(accessToken);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private DateTime GetRefreshTokenExpireTime()
        {
            int expiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]);
            return DateTime.UtcNow.AddDays(expiryDays);
        }

        private DateTime GetAccessTokenExpireTime()
        {
            int expiryMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]);
            return DateTime.UtcNow.AddMinutes(expiryMinutes);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                    ValidateLifetime = false,
                };

                var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is JwtSecurityToken jwtSecurityToken 
                    && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}

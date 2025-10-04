using Application.Models;
using Configuration.Options;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenOptions _jwtTokenOptions;

        public TokenService(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, IOptions<JwtTokenOptions> jwtTokenOptions)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            _userRepository = userRepository;
            _jwtTokenOptions = jwtTokenOptions.Value;
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

            await _refreshTokenRepository.SaveChangesAsync();

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
            if (user == null) throw new ArgumentException("Access or refresh token are invalid.");

            var savedRefreshToken = user.RefreshTokens?.FirstOrDefault(token => token.Token == refreshToken);

            if (savedRefreshToken == null || savedRefreshToken.IsUsed || savedRefreshToken.IsRevoked || savedRefreshToken.Expires < DateTime.UtcNow)
            {
                throw new ArgumentException("Access or refresh token are invalid.");
            }

            savedRefreshToken.IsUsed = true;
            await _refreshTokenRepository.SaveChangesAsync();

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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessTokenExpiration = GetAccessTokenExpireTime();
            var accessToken = new JwtSecurityToken(
                issuer: _jwtTokenOptions.Issuer,
                audience: _jwtTokenOptions.Audience,
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
            int expiryDays = _jwtTokenOptions.RefreshTokenExpirationDays;
            return DateTime.UtcNow.AddDays(expiryDays);
        }

        private DateTime GetAccessTokenExpireTime()
        {
            int expiryMinutes = _jwtTokenOptions.AccessTokenExpirationMinutes;
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
                    ValidIssuer = _jwtTokenOptions.Issuer,
                    ValidAudience = _jwtTokenOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenOptions.Key)),
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

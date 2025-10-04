using Application.Services.Interfaces;
using Configuration.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class PriciplesFromTokenProvider : IPriciplesFromTokenProvider
    {
        private readonly JwtTokenOptions _jwtTokenOptions;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public PriciplesFromTokenProvider(IOptions<JwtTokenOptions> jwtTokenOptions, JwtSecurityTokenHandler tokenHandler)
        {
            _jwtSecurityTokenHandler = tokenHandler;
            _jwtTokenOptions = jwtTokenOptions.Value;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
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

using Application.Models;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ITokenService
    {
        Task<AuthTokens> GenerateTokensAsync(User user);

        Task<AuthTokens> RefreshTokensAsync(string accessToken, string refreshToken);
    }
}

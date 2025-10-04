using Application.Models;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthTokens> AuthenticateAsync(LoginRequest request);

        Task<AuthTokens> RefreshTokensAsync(string accessToken, string refreshToken);
    }
}

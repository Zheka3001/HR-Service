using System.Security.Claims;

namespace Application.Services.Interfaces
{
    public interface IPriciplesFromTokenProvider
    {
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}

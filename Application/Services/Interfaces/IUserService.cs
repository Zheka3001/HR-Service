using Application.Models;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<int> RegisterUserAsync(RegisterUser user);
    }
}

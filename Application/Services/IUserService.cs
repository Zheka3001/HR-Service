using Application.Models;

namespace Application.Services
{
    public interface IUserService
    {
        Task RegisterUserAsync(RegisterUser user);
    }
}

using Application.Models;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        Task RegisterUserAsync(RegisterUser user);
    }
}

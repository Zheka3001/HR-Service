using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDao?> GetByEmailAsync(string email);

        Task<UserDao?> GetByIdAsync(int id);

        Task<List<UserDao>> GetByIdsAsync(IEnumerable<int> userIds);

        Task<bool> UserExistsAsync(string email);

        Task<bool> UserExistsAsync(int id);

        Task AddUserAsync(UserDao user);

        Task SaveChangesAsync();
    }
}

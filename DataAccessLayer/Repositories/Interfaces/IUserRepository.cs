using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDao?> GetByEmailAsync(string email);

        Task<UserDao?> GetByIdAsync(int id);

        Task<List<UserDao>> GetByIdsAsync(IEnumerable<int> userIds);

        Task<bool> UserExistsAsync(string email);

        Task AddUserAsync(UserDao user);

        Task SaveChangesAsync();
    }
}

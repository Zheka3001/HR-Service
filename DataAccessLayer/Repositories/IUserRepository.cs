using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<bool> UserExistsAsync(string email);

        Task AddUserAsync(User user);

        Task UpdateAsync(User user);
    }
}

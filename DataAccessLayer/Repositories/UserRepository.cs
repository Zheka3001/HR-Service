using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(UserDao user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<UserDao?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .SingleOrDefaultAsync(x => x.Login.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<UserDao?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(x => x.RefreshTokens)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<UserDao>> GetByIdsAsync(IEnumerable<int> userIds)
        {
            return await _context.Users
                .Include(x => x.CreatedApplicants)
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Login.Equals(email, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}

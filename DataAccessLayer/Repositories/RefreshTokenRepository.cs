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
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DataContext _context;

        public RefreshTokenRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task InsertTokenAsync(RefreshTokenDao token)
        {
            await _context.RefreshTokens.AddAsync(token);
        }

        public async Task<IEnumerable<RefreshTokenDao>> GetRefreshTokensByUserIdAsync(int userId)
        {
            return await _context.RefreshTokens.Where(token => token.UserId == userId).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

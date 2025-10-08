using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class CheckRepository : ICheckRepository
    {
        private readonly DataContext _context;

        public CheckRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddCheckEventsAsync(IEnumerable<CheckEventDao> checkEvents)
        {
            await _context.CheckEvents.AddRangeAsync(checkEvents);
        }

        public async Task<CheckDao?> GetByIdAsync(int checkId)
        {
            return await _context.Checks
                .Include(c => c.CheckEvents)
                .FirstOrDefaultAsync(c => c.Id == checkId);
        }

        public async Task InsertAsync(CheckDao check)
        {
            await _context.Checks.AddAsync(check);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

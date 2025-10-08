using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;

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

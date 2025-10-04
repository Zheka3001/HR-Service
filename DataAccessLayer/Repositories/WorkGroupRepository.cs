using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class WorkGroupRepository : IWorkGroupRepository
    {
        private readonly DataContext _context;

        public WorkGroupRepository(DataContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task InsertAsync(WorkGroup workGroup)
        {
            await _context.WorkGroups.AddAsync(workGroup);
        }

        public async Task<bool> WorkGroupExistsAsync(int workGroupId)
        {
            return await _context.WorkGroups.AnyAsync(w => w.Id == workGroupId);
        }
    }
}

using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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

        public async Task InsertAsync(WorkGroupDao workGroup)
        {
            await _context.WorkGroups.AddAsync(workGroup);
        }

        public async Task<bool> WorkGroupExistsAsync(int workGroupId)
        {
            return await _context.WorkGroups.AnyAsync(w => w.Id == workGroupId);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }
    }
}

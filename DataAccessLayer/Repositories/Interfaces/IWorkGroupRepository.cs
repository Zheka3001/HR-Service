using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IWorkGroupRepository
    {
        Task InsertAsync(WorkGroup workGroup);

        Task<bool> WorkGroupExistsAsync(int workGroupId);

        Task SaveChangesAsync();

        IDbContextTransaction BeginTransaction();
    }
}
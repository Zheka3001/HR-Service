using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories
{
    public interface IWorkGroupRepository
    {
        Task InsertAsync(WorkGroup workGroup);

        Task<bool> WorkGroupExistsAsync(int workGroupId);

        Task SaveChangesAsync();
    }
}
using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface ICheckRepository
    {
        Task InsertAsync(CheckDao applicant);

        Task SaveChangesAsync();

        Task AddCheckEventsAsync(IEnumerable<CheckEventDao> checkEvents);
    }
}

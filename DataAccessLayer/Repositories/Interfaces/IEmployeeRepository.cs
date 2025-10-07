using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task InsertAsync(EmployeeDao employee);

        Task SaveChangesAsync();
    }
}

using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task InsertAsync(EmployeeDao employee);

        Task<List<int>> SearchEmployeesByFullNameAsync(string fullName);

        Task SaveChangesAsync();
    }
}

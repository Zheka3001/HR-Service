using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DataContext _context;

        public EmployeeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task InsertAsync(EmployeeDao employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> SearchEmployeesByFullNameAsync(string fullName)
        {
            var lowerFullName = fullName.ToLower();

            var result = await _context.Employees
            .Include(e => e.ApplicantInfo)
            .Where(e =>
                (e.ApplicantInfo.FirstName + " " + e.ApplicantInfo.LastName + " " + (e.ApplicantInfo.MiddleName ?? "")).TrimEnd().ToLower().Contains(lowerFullName))
            .Select(e => e.Id)
            .ToListAsync();

            return result;
        }
    }
}

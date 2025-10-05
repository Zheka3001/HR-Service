using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IApplicantRepository
    {
        Task InsertAsync(Applicant applicant);

        Task SaveChangesAsync();
    }
}
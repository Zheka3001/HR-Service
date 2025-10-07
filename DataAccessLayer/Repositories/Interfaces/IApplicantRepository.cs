using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IApplicantRepository
    {
        Task InsertAsync(ApplicantDao applicant);

        Task<ApplicantDao?> GetByIdAsync(int id);

        Task SaveChangesAsync();

        Task<ApplicantSearchResultDao> SearchAsync(ApplicantSearchRequestDao searchRequest);

        Task DeleteAsync(int applicantId);
    }
}
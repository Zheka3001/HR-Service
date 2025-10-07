using Application.Models;
using Model.Search;

namespace Application.Services.Interfaces
{
    public interface IApplicantService
    {
        Task<int> CreateApplicantAsync(CreateApplicantRequest request, int creatorId);
        Task<QueryResultByCriteria<ApplicantSearchResult>> SearchAsync(ApplicantSearchCriteria searchCriteria, int currentUserId);
        Task<int> TransferToEmployeeAsync(int userId, int applicantId);
        Task UpdateApplicantAsync(UpdateApplicantRequest request, int initiatorId);
    }
}
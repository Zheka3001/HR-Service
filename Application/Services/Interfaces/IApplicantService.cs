using Application.Models;
using Model.Search;

namespace Application.Services.Interfaces
{
    public interface IApplicantService
    {
        Task<int> CreateApplicantAsync(CreateApplicantRequest request, int creatorId);
        Task<QueryResultByCriteria<ApplicantSearchResult>> SearchAsync(ApplicantSearchCriteria searchCriteria, int currentUserId);
        Task UpdateApplicantAsync(UpdateApplicantRequest request, int initiatorId);
    }
}
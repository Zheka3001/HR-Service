using Application.Models;

namespace Application.Services.Interfaces
{
    public interface IApplicantService
    {
        Task<int> CreateApplicantAsync(CreateApplicantRequest request, int creatorId);

        Task UpdateApplicantAsync(UpdateApplicantRequest request, int initiatorId);
    }
}
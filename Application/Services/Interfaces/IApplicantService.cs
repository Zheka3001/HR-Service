using Application.Models;

namespace Application.Services.Interfaces
{
    public interface IApplicantService
    {
        Task<CreateApplicantResponse> CreateApplicantAsync(CreateApplicantRequest request, int creatorId);
    }
}
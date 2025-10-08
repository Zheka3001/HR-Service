using Application.Models;

namespace Application.Services.Interfaces
{
    public interface ICheckService
    {
        Task<CheckResult> GetCheckResultsAsync(int checkId);
        Task<int> StartCheckAsync(string fullName, int initiatorId);
    }
}

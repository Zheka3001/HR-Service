namespace Application.Services.Interfaces
{
    public interface ICheckService
    {
        Task<int> StartCheckAsync(string fullName, int initiatorId);
    }
}

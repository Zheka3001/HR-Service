using Application.Models;

namespace Application.Services.Interfaces
{
    public interface IWorkGroupService
    {
        Task<int> AddAsync(CreateWorkGroup workGroup);
        Task MoveHrsAsync(MoveHrsRequest moveHrsRequest);
    }
}
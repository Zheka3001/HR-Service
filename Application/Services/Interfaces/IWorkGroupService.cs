using Application.Models;

namespace Application.Services.Interfaces
{
    public interface IWorkGroupService
    {
        Task InsertAsync(CreateWorkGroup workGroup);
        Task MoveHrsAsync(MoveHrsRequest moveHrsRequest);
    }
}
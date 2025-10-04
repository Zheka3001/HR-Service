using Application.Models;

namespace Application.Services
{
    public interface IWorkGroupService
    {
        Task InsertAsync(CreateWorkGroup workGroup);
    }
}
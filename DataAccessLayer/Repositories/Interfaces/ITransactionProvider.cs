using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface ITransactionProvider
    {
        IDbContextTransaction BeginTransaction();
    }
}

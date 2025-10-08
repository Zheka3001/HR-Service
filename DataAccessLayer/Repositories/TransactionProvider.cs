using DataAccessLayer.Data;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccessLayer.Repositories
{
    internal class TransactionProvider : ITransactionProvider
    {
        private readonly DataContext _context;

        public TransactionProvider(DataContext context)
        {
            _context = context;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}

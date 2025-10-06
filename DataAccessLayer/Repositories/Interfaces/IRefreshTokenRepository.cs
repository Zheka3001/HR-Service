using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task InsertTokenAsync(RefreshTokenDao token);

        Task<IEnumerable<RefreshTokenDao>> GetRefreshTokensByUserIdAsync(int userId);

        Task SaveChangesAsync();
    }
}

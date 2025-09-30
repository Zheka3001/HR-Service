using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task InsertTokenAsync(RefreshToken token);

        Task<IEnumerable<RefreshToken>> GetRefreshTokensByUserIdAsync(int userId);

        Task UpdateRefreshToken(RefreshToken token);
    }
}

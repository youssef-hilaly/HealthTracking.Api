using HealthTracking.Entity.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.DataService.IRepository
{
    public interface IRefreshTokensRepository : IGenericRepository<RefreshToken> 
    {
        Task<RefreshToken> GetByRefreshToken(string refreshToken);

        Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken);
    }
}

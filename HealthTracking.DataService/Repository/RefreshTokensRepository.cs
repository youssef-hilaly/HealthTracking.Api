using HealthTracking.DataService.Data;
using HealthTracking.DataService.IRepository;
using HealthTracking.Entity.DbSet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.DataService.Repository
{
    public class RefreshTokensRepository : GenericRepository<RefreshToken> ,IRefreshTokensRepository
    {
        public RefreshTokensRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public override async Task<IEnumerable<RefreshToken>> GetAll()
        {
            try
            {
                return await dbSet.Where(x => x.status == 1)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetAll methods has generated an error", typeof(RefreshTokensRepository));
                return null;
            }
        }

        public async Task<RefreshToken> GetByRefreshToken(string refreshToken)
        {
            try
            {
                return await dbSet.Where(x => x.Token.ToLower() == refreshToken.ToLower())
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetByRefreshTokens has generated an error", typeof(RefreshTokensRepository));
                return null;
            }
        }

        public async Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken)
        {
            try
            {
                var token = await dbSet.Where(x => x.Token.ToLower() == refreshToken.Token.ToLower())
                    .FirstOrDefaultAsync();

                if(token == null)
                    return false;

                token.IsUsed = refreshToken.IsUsed;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} MarkRefreshTokenAsUsed has generated an error", typeof(RefreshTokensRepository));
                return false;
            }
        }
    }
}

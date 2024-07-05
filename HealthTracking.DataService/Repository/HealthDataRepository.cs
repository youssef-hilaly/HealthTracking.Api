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
    public class HealthDataRepository : GenericRepository<HealthData>, IHealthDataRepository
    {
        public HealthDataRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public override async Task<IEnumerable<HealthData>> GetAll()
        {
            try
            {
                return await dbSet.Where(u => u.status == 1)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All methods has generated an error", typeof(HealthDataRepository));
                return null;
            }
        }

        public async Task<List<HealthData>> GetUserHealthData(Guid userId)
        {
            try
            {
                return await dbSet.Where(h => h.userId == userId).ToListAsync();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateHealthData(HealthData healthData)
        {
            try
            {
                var existHealthData = await dbSet.Where(h => h.status == 1 && h.Id == healthData.Id)
                    .FirstOrDefaultAsync();

                if (existHealthData == null) return false;

                existHealthData.Wieght = healthData.Wieght;
                existHealthData.Height = healthData.Height;
                existHealthData.BooldType = healthData.BooldType;
                existHealthData.Race = healthData.Race;
                existHealthData.UseGlasses = healthData.UseGlasses;
                existHealthData.UpdateDate = DateTime.UtcNow;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} UpdateUserProfile methods has generated an error", typeof(UsersRepository));
                return false;
            }
        }



    }

}

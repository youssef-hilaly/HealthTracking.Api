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
    public class UsersRepository : GenericRepository<User>, IUsersRepository
    {
        public UsersRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            try
            {
                var users = await dbSet.Where(u => u.status == 1)
                    .AsNoTracking()
                    .ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All methods has generated an error", typeof(UsersRepository));
                return null;
            }
        }

        // TODO Delete User
        public override Task<bool> Delete(Guid id, string userId)
        {
            return base.Delete(id, userId);
        }
    }
}

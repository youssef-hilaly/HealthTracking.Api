using HealthTracking.DataService.Data;
using HealthTracking.DataService.IRepository;
using HealthTracking.Entity.DbSet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
                return await dbSet.Where(u => u.status == 1)
                    .AsNoTracking()
                    .ToListAsync();
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

        public async Task<bool> UpdateUserProfile(User user)
        {
            try
            {
                var existUser = await dbSet.Where(u => u.status == 1 && u.Id == user.Id)
                    .FirstOrDefaultAsync();

                if (existUser == null) return false;

                existUser.FirstName = user.FirstName;
                existUser.LastName = user.LastName;
                existUser.Address = user.Address;
                existUser.Country = user.Country;
                existUser.PhoneNumber = user.PhoneNumber;
                existUser.Sex = user.Sex;
                existUser.UpdateDate = DateTime.UtcNow;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} UpdateUserProfile methods has generated an error", typeof(UsersRepository));
                return false;
            }
        }

        public async Task<User> GetByIdentityId(Guid identityId)
        {
            try
            {
                return await dbSet.Where(u => u.status == 1 && u.IdentityId == identityId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} UpdateUserProfile methods has generated an error", typeof(UsersRepository));
                return null;
            }
        }

        public async Task<User> FindByEmail(string Email)
        {
            try
            {
                return await _context.Users.Where(u => u.Email == Email).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} FindByEmail methods has generated an error", typeof(UsersRepository));
                return null;
            }
        }
    }
}

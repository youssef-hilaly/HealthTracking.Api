using HealthTracking.Entity.DbSet;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.DataService.IRepository
{
    public interface IUsersRepository : IGenericRepository<User>
    {
        Task<bool> UpdateUserProfile(User user);
        Task<User> GetByIdentityId(Guid identityId);
        Task<User> FindByEmail(string Email);
    }
}

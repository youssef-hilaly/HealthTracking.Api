using HealthTracking.Entity.DbSet;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthTracking.DataService.Data
{
    public class AppDbContext : IdentityDbContext
    {
        // add-migration init -StartupProject HealthTracking.Api  // for refrencing
        public virtual DbSet<User> Users {  get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens {  get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}

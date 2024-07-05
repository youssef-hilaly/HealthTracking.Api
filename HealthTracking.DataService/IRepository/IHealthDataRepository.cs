using HealthTracking.Entity.DbSet;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.DataService.IRepository
{
    public interface IHealthDataRepository : IGenericRepository<HealthData>
    {
        Task<List<HealthData>> GetUserHealthData(Guid userId);
        Task<bool> UpdateHealthData(HealthData healthData);
    }
}

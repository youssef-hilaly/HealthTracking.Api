using HealthTracking.DataService.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.DataService.IConfigration
{
    public interface IUnitOfWork
    {
        IUsersRepository Users{ get; }
        IRefreshTokensRepository RefreshTokens { get; }
        IHealthDataRepository HealthData { get; }

        Task CompleteAsync();
    }
}

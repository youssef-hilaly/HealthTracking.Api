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

        Task CompleteAsync();
    }
}

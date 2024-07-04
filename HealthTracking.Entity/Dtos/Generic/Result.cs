using HealthTracking.Entity.Dtos.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.Entity.Dtos.Generic
{
    public class Result<T> // single item return
    {
        public T Content { get; set; }
        public Error Error { get; set; }
        public bool IsSuccess => Error == null;
        public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
    }
}

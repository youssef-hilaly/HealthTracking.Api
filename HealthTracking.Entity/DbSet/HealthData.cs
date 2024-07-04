using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.Entity.DbSet
{
    public class HealthData : BaseEntity
    {
        public decimal Height {  get; set; }
        public decimal Wieght { get; set; }
        public string BooldType { get; set; } // TODO make it as Enum
        public string Race { get; set; }
        public bool UseGlasses { get; set; }
    }
}

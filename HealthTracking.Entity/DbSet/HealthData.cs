using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.Entity.DbSet
{
    public class HealthData : BaseEntity
    {
        public decimal Height {  get; set; }
        public decimal Wieght { get; set; }
        public decimal BloodPresure { get; set; }
        public decimal BooldSugerLevel { get; set; }
        public string BooldType { get; set; } // TODO make it as Enum
        public string Race { get; set; }
        public bool UseGlasses { get; set; }

        [ForeignKey(nameof(User))]
        public Guid userId { get; set; }
        public User User { get; set; }
    }
}

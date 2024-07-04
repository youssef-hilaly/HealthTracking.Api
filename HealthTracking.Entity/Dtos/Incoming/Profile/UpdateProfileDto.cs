using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.Entity.Dtos.Incoming.Profile
{
    public class UpdateProfileDto
    {
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Sex { get; set; }
    }
}

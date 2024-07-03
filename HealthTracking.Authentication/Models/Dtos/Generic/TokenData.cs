using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.Authentication.Models.Dtos.Generic
{
    public class TokenData
    {
        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}

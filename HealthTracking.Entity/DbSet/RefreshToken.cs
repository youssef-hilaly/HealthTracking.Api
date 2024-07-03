using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.Entity.DbSet
{
    public class RefreshToken : BaseEntity
    {
        public string UserId { get; set; } // User Id when logged in
        public string Token { get; set; } // 
        public string JwtId { get; set; } // came from JwtRegisteredClaimNames.Jti 
        public bool IsUsed { get; set; } // To make sure the token is only used once
        public bool IsRevoked { get; set; } // Make sure they are valid
        public DateTime ExpiryDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

    }
}

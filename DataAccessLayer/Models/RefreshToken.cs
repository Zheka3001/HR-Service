using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public bool IsRevoked { get; set; } = false;

        public bool IsUsed { get; set; } = false;

        // Foreign key for User
        public int UserId { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}

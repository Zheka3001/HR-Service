using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class UserDao
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? MiddleName { get; set; }

        public RoleDao Role { get; set; }

        public string Login { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        // Relationship
        public ICollection<RefreshTokenDao> RefreshTokens { get; set; }

        public int? WorkGroupId { get; set; }
        public WorkGroupDao? WorkGroup { get; set; }

        public ICollection<ApplicantDao> CreatedApplicants { get; set; }
    }
}

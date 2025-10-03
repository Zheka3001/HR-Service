using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class WorkGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // Participants (Users and Applicants) in the group
        public ICollection<User> Users { get; set; } // HR Users

        public ICollection<Applicant> Applicants { get; set; }
    }
}

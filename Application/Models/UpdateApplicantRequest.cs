using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class UpdateApplicantRequest
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? MiddleName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Country { get; set; }

        public DateTime DateOfBirth { get; set; }

        public WorkSchedule WorkSchedule { get; set; }

        public ICollection<SocialNetwork> SocialNetworks { get; set; }
    }
}

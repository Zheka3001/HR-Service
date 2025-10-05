using DataAccessLayer.Models;

namespace Application.Models
{
    public class CreateApplicantRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? MiddleName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Country { get; set; }

        public DateTime DateOfBirth { get; set; }

        public WorkSchedule WorkSchedule { get; set; }

        public int? WorkGroupId { get; set; }

        public int? CreatorId { get; set; }

        public ICollection<CreateSocialNetworkInfoRequest> CreateSocialNetworkInfoRequests { get; set; }
    }
}
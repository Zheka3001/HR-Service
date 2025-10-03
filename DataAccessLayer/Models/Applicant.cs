namespace DataAccessLayer.Models
{
    public class Applicant
    {
        public int Id { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public WorkSchedule WorkSchedule { get; set; }

        // Relationships
        public int ApplicantInfoId { get; set; }
        public ApplicantInfo ApplicantInfo { get; set; }

        public int WorkGroupId { get; set; }
        public WorkGroup WorkGroup { get; set; }
    }
}
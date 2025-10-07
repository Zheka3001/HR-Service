namespace DataAccessLayer.Models
{
    public class ApplicantDao
    {
        public int Id { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public WorkScheduleDao WorkSchedule { get; set; }

        // Relationships
        public int ApplicantInfoId { get; set; }
        public ApplicantInfoDao ApplicantInfo { get; set; }

        public int WorkGroupId { get; set; }
        public WorkGroupDao WorkGroup { get; set; }

        public int CreatedById { get; set; }
        public UserDao CreatedBy { get; set; }

        public int LastUpdatedById { get; set; }

        public UserDao LastUpdatedBy { get; set; }
    }
}
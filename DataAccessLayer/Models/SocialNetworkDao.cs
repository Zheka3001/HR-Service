namespace DataAccessLayer.Models
{
    public class SocialNetworkDao
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public SocialNetworkTypeDao Type { get; set; }

        public DateTime CreateDate { get; set; }

        public int ApplicantInfoId { get; set; }

        public ApplicantInfoDao ApplicantInfo { get; set; }
    }
}
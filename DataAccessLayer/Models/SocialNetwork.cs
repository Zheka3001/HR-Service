namespace DataAccessLayer.Models
{
    public class SocialNetwork
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public SocialNetworkType Type { get; set; }

        public DateTime CreateDate { get; set; }

        public int ApplicantInfoId { get; set; }

        public ApplicantInfo ApplicantInfo { get; set; }
    }
}
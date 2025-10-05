using DataAccessLayer.Models;

namespace Application.Models
{
    public class CreateSocialNetworkInfoRequest
    {
        public string UserName { get; set; }

        public SocialNetworkType Type { get; set; }
    }
}
using DataAccessLayer.Models;

namespace WebAPI.DTOs
{
    public class CreateSocialNetworkInfoRequestDto
    {
        public string UserName { get; set; }

        public SocialNetworkType Type { get; set; }
    }
}

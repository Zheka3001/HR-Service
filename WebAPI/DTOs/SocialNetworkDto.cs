using DataAccessLayer.Models;

namespace WebAPI.DTOs
{
    public class SocialNetworkDto
    {
        public string UserName { get; set; }

        public SocialNetworkTypeDto Type { get; set; }
    }
}

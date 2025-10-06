using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class SocialNetwork
    {
        public string UserName { get; set; }

        public SocialNetworkType Type { get; set; }
    }
}

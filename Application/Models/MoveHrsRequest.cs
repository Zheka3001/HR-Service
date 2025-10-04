using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class MoveHrsRequest
    {
        public int WorkGroupId { get; set; }

        public IEnumerable<int> UserIds { get; set; }
    }
}

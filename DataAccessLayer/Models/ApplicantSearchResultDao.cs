using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class ApplicantSearchResultDao
    {
        public IEnumerable<ApplicantDao> SearchedItems { get; init; }

        public SearchedItemsRangeDao ItemsRange { get; init; }
    }
}

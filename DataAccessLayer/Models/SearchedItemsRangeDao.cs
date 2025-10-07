using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class SearchedItemsRangeDao
    {
        public int TotalSearchedItems { get; init; }

        public int PageSize { get; init; }

        public int PageNumber { get; init; }

        public int TotalPages { get; init; }
    }
}

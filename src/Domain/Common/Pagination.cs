using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public class Pagination
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        internal bool HasPreviousPage => PageNumber > 1;
        internal bool HasNextPage => PageNumber < TotalPages;
    }
}

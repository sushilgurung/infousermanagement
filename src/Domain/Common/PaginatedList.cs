using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public class PaginatedList<T>
    {
        public PaginatedList() { }
        public IEnumerable<T> Items { get; }

        public Pagination Pagination { get; set; }

        public PaginatedList(List<T> items, int pageNumber, int pageSize, int count)
        {
            Pagination = new Pagination()
            {
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                TotalItems = count,
                PageSize = pageSize,
            };
            Items = items;
        }

        public PaginatedList(IEnumerable<T> items, int pageNumber, int pageSize, int count)
        {
            Pagination = new Pagination()
            {
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                TotalItems = count,
                PageSize = pageSize,
            };
            Items = items.ToList();
        }

        public static PaginatedList<T> Create(IReadOnlyList<T> data, int pageSize, int pageNumber, int totalCount)
        {
            return new PaginatedList<T>(data, pageNumber, pageSize, totalCount);
        }

    }
}

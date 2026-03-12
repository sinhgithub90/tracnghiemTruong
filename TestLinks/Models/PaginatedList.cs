
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestLinks
{
    public class PaginatedList<T> : List<T>, IPaginatedList
    {
        public int TotalPage { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPage = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;

            this.AddRange(items);
        }


        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPage);
            }
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source,
            int pageIndex, int pageSize = 10)
        {
            var count = await source.CountAsync();
            var items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }


    }
}

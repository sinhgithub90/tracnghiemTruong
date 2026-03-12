using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestLinks
{
    public interface IPaginatedList
    {
        int TotalPage { get; }
        int PageIndex { get; }
        int PageSize { get; }
        int Count { get;  }
        int TotalCount { get; }

        bool HasNextPage { get; }
        bool HasPreviousPage { get;  }
    }
}

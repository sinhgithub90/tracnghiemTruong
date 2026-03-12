using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestLinks.Models
{
    public class PaginatedNavigatorViewModel
    {
        public IPaginatedList PaginatedList { get; set; }
        public Dictionary<string, string> Filters { get; set; }
    }
}

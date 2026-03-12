
using System.Collections.Generic;
using TestLinks;

public class PaginatedViewModel<T>
        where T : class
{
    public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
    public PaginatedList<T> Items { get; set; }
}
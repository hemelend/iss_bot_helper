using System.Collections.Generic;

namespace SFBot
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }

        public int TotalCount { get; set; }
    }
}
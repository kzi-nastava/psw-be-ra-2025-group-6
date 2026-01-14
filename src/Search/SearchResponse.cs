using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;


namespace Search
{
    public class SearchResponse
    {
        public IReadOnlyCollection<SearchItemDto> Items { get; }

        public SearchResponse(IEnumerable<SearchItemDto> items)
        {
            Items = items.ToList();
        }
    }

}

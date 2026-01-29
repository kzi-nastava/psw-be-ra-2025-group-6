using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Search
{
    public class SearchRequest
    {
        public string? Query { get; init; } = string.Empty;

        public IReadOnlyCollection<SearchEntityType> Types { get; init; }
            = Array.Empty<SearchEntityType>();
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class SearchItemDto
    {
        public long Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public SearchEntityType Type { get; init; }

        public string? Description { get; init; }
        public string Url { get; init; } = string.Empty;
    }

}

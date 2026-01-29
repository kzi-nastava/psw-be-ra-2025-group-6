using System.Collections.Generic;
namespace Explorer.Global.Search;
public class SearchResponse
{
    public IReadOnlyCollection<SearchItemDto> Items { get; }

    public SearchResponse(IEnumerable<SearchItemDto> items)
    {
        Items = items.ToList();
    }
}


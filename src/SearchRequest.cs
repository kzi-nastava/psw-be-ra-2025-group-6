using System;

namespace Explorer.Global.Search;
public class SearchRequest
{
    public string Query { get; init; } = string.Empty;

    public IReadOnlyCollection<SearchEntityType> Types { get; init; }
        = Array.Empty<SearchEntityType>();
}


using System;
namespace Explorer.Global.Search;
{
public class SearchItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public SearchEntityType Type { get; init; }

    public string? Description { get; init; }
    public string Url { get; init; } = string.Empty;
}
}

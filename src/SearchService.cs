using System.Collections.Generic;
using System.Threading.Tasks;
namespace Explorer.Global.Search;

public class SearchService : ISearchService
{
    private readonly IBlogSearchService _blogSearch;
    private readonly ITourSearchService _tourSearch;
    private readonly IUserSearchService _userSearch;

    public SearchService(
        IBlogSearchService blogSearch,
        ITourSearchService tourSearch,
        IUserSearchService userSearch)
    {
        _blogSearch = blogSearch;
        _tourSearch = tourSearch;
        _userSearch = userSearch;
    }

    public async Task<SearchResponse> SearchAsync(
        SearchRequest request,
        ClaimsPrincipal user)
    {
        var results = new List<SearchItemDto>();

        var searchAll = !request.Types.Any();

        if (searchAll || request.Types.Contains(SearchEntityType.Blog))
            results.AddRange(await _blogSearch.SearchAsync(request.Query, user));

        if (searchAll || request.Types.Contains(SearchEntityType.Tour))
            results.AddRange(await _tourSearch.SearchAsync(request.Query, user));

        if (searchAll || request.Types.Contains(SearchEntityType.User))
            results.AddRange(await _userSearch.SearchAsync(request.Query, user));

        return new SearchResponse(results);
    }
}


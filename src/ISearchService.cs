using System.Threading.Tasks;
namespace Explorer.Global.Search;
public interface ISearchService
{
    Task<SearchResponse> SearchAsync(
        SearchRequest request,
        ClaimsPrincipal user);
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Explorer.Blog.API.Public.Administration;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Services;
using Explorer.Tours.API.Public;
using Shared;


namespace Search
{
    public class SearchService : ISearchService
    {
        private readonly IBlogSearchService _blogSearch;
        private readonly ITourSearchService _tourSearch;
        private readonly IUserSearchService _userSearch;
        private readonly IClubSearchService _clubSearch;

        public SearchService(
            IBlogSearchService blogSearch,
            ITourSearchService tourSearch,
            IUserSearchService userSearch,
            IClubSearchService clubSearch)
        {
            _blogSearch = blogSearch;
            _tourSearch = tourSearch;
            _userSearch = userSearch;
            _clubSearch=clubSearch;
        }

        public async Task<SearchResponse> SearchAsync(
            SearchRequest request,
            ClaimsPrincipal user, long personId, string userRole)
        {
            var results = new List<SearchItemDto>();

            var searchAll = !request.Types.Any();

            if (searchAll || request.Types.Contains(SearchEntityType.Blog))
                results.AddRange(await _blogSearch.SearchAsync(request.Query, user, personId, userRole));

            if (searchAll || request.Types.Contains(SearchEntityType.Tour))
                results.AddRange(await _tourSearch.SearchAsync(request.Query, user, personId,userRole));

            if (searchAll || request.Types.Contains(SearchEntityType.User))
                results.AddRange(await _userSearch.SearchAsync(request.Query, user, personId,userRole));

            if (searchAll || request.Types.Contains(SearchEntityType.Club))
                results.AddRange(await _clubSearch.SearchAsync(request.Query, user, personId, userRole));

            return new SearchResponse(results);
        }
    }

}

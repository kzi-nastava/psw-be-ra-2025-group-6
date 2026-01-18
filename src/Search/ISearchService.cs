using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    public interface ISearchService
    {
        Task<SearchResponse> SearchAsync(
            SearchRequest request,
            ClaimsPrincipal user, long personId, string userRole);
    }

}

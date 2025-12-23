using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Shared;


namespace Explorer.Blog.API.Public.Administration
{ 
public interface IBlogSearchService
{
    Task<IReadOnlyCollection<SearchItemDto>> SearchAsync(
        string query,
        ClaimsPrincipal user, long personId);
}
}

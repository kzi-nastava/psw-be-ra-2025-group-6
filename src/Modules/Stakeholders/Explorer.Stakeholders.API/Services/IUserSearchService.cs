using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Services
{
    public interface IUserSearchService
    {
        Task<IReadOnlyCollection<SearchItemDto>> SearchAsync(
            string query,
            ClaimsPrincipal user, long personId);
    }
}

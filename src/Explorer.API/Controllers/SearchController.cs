using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Security.Claims;
using System.Threading.Tasks;
using Search;
using Explorer.Stakeholders.Infrastructure.Authentication;

namespace Explorer.API.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string query,
            [FromQuery] SearchEntityType[] types)
        {
            var personId = User.PersonId();

            var request = new SearchRequest
            {
                Query = query,
                Types = types
            };

            var response = await _searchService.SearchAsync(request, User, personId);

            return Ok(response.Items);
        }
    }
}


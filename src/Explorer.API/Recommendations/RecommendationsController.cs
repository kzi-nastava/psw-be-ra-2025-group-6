using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Explorer.API.Recommendations;

[ApiController]
[Route("api/recommendations")]
public class RecommendationsController : ControllerBase
{
    private readonly InMemoryRecommendationsStore _store;
    private readonly RecommendationsOptions _options;
    private readonly IWebHostEnvironment _environment;

    public RecommendationsController(
        InMemoryRecommendationsStore store,
        IOptions<RecommendationsOptions> options,
        IWebHostEnvironment environment)
    {
        _store = store;
        _options = options.Value;
        _environment = environment;
    }

    [Authorize]
    [HttpGet]
    public ActionResult<RecommendationsResponse> Get()
    {
        var userKey = ResolveUserKey();
        var tourIds = GetTourIds();

        if (_environment.IsDevelopment() && tourIds.Count > 0)
        {
            _store.SeedNew(userKey, tourIds);
        }

        var newIds = _store.GetNewTourIds(userKey);

        return Ok(new RecommendationsResponse
        {
            TourIds = tourIds,
            NewTourIds = newIds
        });
    }

    [Authorize]
    [HttpPut("ack")]
    public IActionResult Ack()
    {
        var userKey = ResolveUserKey();
        _store.AckAll(userKey);
        return NoContent();
    }

    [Authorize]
    [HttpPut("seen/{tourId:int}")]
    public IActionResult Seen(int tourId)
    {
        var userKey = ResolveUserKey();
        _store.MarkSeen(userKey, tourId);
        return NoContent();
    }

    private List<int> GetTourIds()
    {
        if (!_environment.IsDevelopment())
        {
            return new List<int>();
        }

        return _options.TourIds
            .Where(id => id > 0)
            .Distinct()
            .ToList();
    }

    private string ResolveUserKey()
    {
        var key = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub")
                  ?? User.Identity?.Name
                  ?? "anon";

        return key;
    }

    public sealed class RecommendationsResponse
    {
        [JsonPropertyName("tourIds")]
        public List<int> TourIds { get; set; } = new();

        [JsonPropertyName("newTourIds")]
        public List<int> NewTourIds { get; set; } = new();
    }
}

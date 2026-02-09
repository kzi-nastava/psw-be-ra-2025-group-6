using System.Collections.Generic;

namespace Explorer.API.Recommendations;

public sealed class RecommendationsOptions
{
    public List<int> TourIds { get; set; } = new();
}

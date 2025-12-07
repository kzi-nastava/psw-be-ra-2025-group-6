using Explorer.BuildingBlocks.Core.Domain;
using System.Text.Json;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Infrastructure.Database.Entities;

public class TourExecutionEntity
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public long TouristId { get; set; }
    public string Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string InitialPositionJson { get; set; }
    public string ExecutionKeyPointsJson { get; set; }
    public DateTime LastActivity { get; set; }
}

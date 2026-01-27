using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourCheckpointPlan : Entity
{
    public long UserId { get; private set; }
    public long PlannerItemId { get; private set; }
    public long KeyPointId { get; private set; }
    public DateTime PlannedAt { get; private set; }

    private TourCheckpointPlan() { }

    public TourCheckpointPlan(long userId, long plannerItemId, long keyPointId, DateTime plannedAt)
    {
        UserId = userId;
        PlannerItemId = plannerItemId;
        KeyPointId = keyPointId;
        PlannedAt = plannedAt;
        Validate();
    }

    public void UpdatePlannedAt(DateTime plannedAt)
    {
        PlannedAt = plannedAt;
        Validate();
    }

    private void Validate()
    {
        if (UserId <= 0) throw new ArgumentException("Invalid UserId.");
        if (PlannerItemId <= 0) throw new ArgumentException("Invalid PlannerItemId.");
        if (KeyPointId <= 0) throw new ArgumentException("Invalid KeyPointId.");
        if (PlannedAt == default) throw new ArgumentException("Invalid PlannedAt.");
    }
}

using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public enum PublicEntityType
{
    KeyPoint,
    Facility
}

public enum RequestStatus
{
    Pending,
    Approved,
    Rejected
}

public class PublicEntityRequest : Entity
{
    public long AuthorId { get; init; }
    public PublicEntityType EntityType { get; init; }
    public long EntityId { get; init; }
    public RequestStatus Status { get; private set; }
    public DateTime RequestedAt { get; init; }
    public DateTime? ProcessedAt { get; private set; }
    public long? ProcessedByAdminId { get; private set; }
    public string? AdminComment { get; private set; }

    private PublicEntityRequest() { }

    public PublicEntityRequest(long authorId, PublicEntityType entityType, long entityId)
    {
        if (authorId == 0) throw new ArgumentException("Invalid AuthorId");
        if (entityId == 0) throw new ArgumentException("Invalid EntityId");

        AuthorId = authorId;
        EntityType = entityType;
        EntityId = entityId;
        Status = RequestStatus.Pending;
        RequestedAt = DateTime.UtcNow;
    }

    public void Approve(long adminId)
    {
        if (Status != RequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be approved.");
        
        Status = RequestStatus.Approved;
        ProcessedAt = DateTime.UtcNow;
        ProcessedByAdminId = adminId;
    }

    public void Reject(long adminId, string comment)
    {
        if (Status != RequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be rejected.");
        
        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Comment is required when rejecting a request.");

        Status = RequestStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
        ProcessedByAdminId = adminId;
        AdminComment = comment;
    }
}

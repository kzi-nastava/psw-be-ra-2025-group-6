namespace Explorer.Tours.API.Dtos;

public enum PublicEntityTypeDto
{
    KeyPoint,
    Facility
}

public enum RequestStatusDto
{
    Pending,
    Approved,
    Rejected
}

public class PublicEntityRequestDto
{
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public PublicEntityTypeDto EntityType { get; set; }
    public long EntityId { get; set; }
    public RequestStatusDto Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public long? ProcessedByAdminId { get; set; }
    public string? AdminComment { get; set; }
}

public class CreatePublicEntityRequestDto
{
    public PublicEntityTypeDto EntityType { get; set; }
    public long EntityId { get; set; }
}

public class ProcessRequestDto
{
    public string? AdminComment { get; set; }
}

public class RejectRequestDto
{
    public string Comment { get; set; }
}

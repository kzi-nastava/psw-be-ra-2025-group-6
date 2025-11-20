namespace Explorer.Tours.API.Dtos;

public class TouristEquipmentDto
{
    public long Id { get; set; }
    public long PersonId { get; set; }
    public long EquipmentId { get; set; }

    // Enrichment for UI
    public string? Name { get; set; }
    public string? Description { get; set; }
}

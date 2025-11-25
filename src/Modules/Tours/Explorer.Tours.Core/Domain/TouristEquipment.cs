using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TouristEquipment : Entity
{
    public long PersonId { get; init; }
    public long EquipmentId { get; init; }

    public TouristEquipment(long personId, long equipmentId)
    {
        PersonId = personId;
        EquipmentId = equipmentId;
        Validate();
    }

    private void Validate()
    {
        if (PersonId == 0) throw new ArgumentException("Invalid PersonId");
        if (EquipmentId == 0) throw new ArgumentException("Invalid EquipmentId");
    }
}

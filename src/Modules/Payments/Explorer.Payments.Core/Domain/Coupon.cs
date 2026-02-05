using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain;

public class Coupon : Entity
{
    public string Code { get; private set; }
    public int DiscountPercent { get; private set; }
    public DateTime? ValidUntil { get; private set; }
    public long? TourId { get; private set; }
    public long AuthorId { get; init; }
    public DateTime CreatedAt { get; init; }

    private Coupon() { }

    public Coupon(long authorId, int discountPercent, long? tourId = null, DateTime? validUntil = null)
    {
        if (authorId == 0) throw new ArgumentException("Invalid AuthorId");
        if (discountPercent < 1 || discountPercent > 100)
            throw new ArgumentException("Discount percent must be between 1 and 100");

        AuthorId = authorId;
        Code = GenerateCode();
        DiscountPercent = discountPercent;
        TourId = tourId;
        ValidUntil = validUntil;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(int discountPercent, long? tourId, DateTime? validUntil)
    {
        if (discountPercent < 1 || discountPercent > 100)
            throw new ArgumentException("Discount percent must be between 1 and 100");

        DiscountPercent = discountPercent;
        TourId = tourId;
        ValidUntil = validUntil;
    }

    public bool IsValid()
    {
        if (ValidUntil.HasValue && ValidUntil.Value < DateTime.UtcNow)
            return false;

        return true;
    }

    public bool AppliesToTour(long tourId)
    {
        return !TourId.HasValue || TourId.Value == tourId;
    }

    private static string GenerateCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

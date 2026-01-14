using Explorer.Payments.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database;

public class PaymentsContext : DbContext
{
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }
    public DbSet<Bundle> Bundles { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<PaymentRecord> PaymentRecords { get; set; }

    public PaymentsContext(DbContextOptions<PaymentsContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payments");

        ConfigurePayments(modelBuilder);
    }

    private static void ConfigurePayments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShoppingCart>()
            .OwnsMany(s => s.Items);

        modelBuilder.Entity<Bundle>()
            .Property(b => b.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Bundle>()
            .Property(b => b.TourIds)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList())
            .HasColumnName("TourIds");

        modelBuilder.Entity<Sale>()
            .Property(s => s.TourIds)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList())
            .HasColumnName("TourIds");

        modelBuilder.Entity<Coupon>()
            .HasIndex(c => c.Code)
            .IsUnique();
    }
}

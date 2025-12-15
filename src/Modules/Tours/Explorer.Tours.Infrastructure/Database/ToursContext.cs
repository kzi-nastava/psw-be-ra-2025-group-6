using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Journal> Journals { get; set; }

    public DbSet<AnnualAward> AnnualAwards { get; set; }

    public DbSet<TouristEquipment> TouristEquipment { get; set; }

    public DbSet<Tour> Tours { get; set; }
    public DbSet<Monument> Monuments { get; set; }
    public DbSet<Meetup> Meetups { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<Facility> Facility { get; set; }

    public DbSet<KeyPoint> KeyPoints { get; set; }

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        ConfigureTouristEquipment(modelBuilder);
        ConfigureShoppingCart(modelBuilder);
    }

    private static void ConfigureShoppingCart(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShoppingCart>()
            .OwnsMany(s => s.Items);

        modelBuilder.Entity<Tour>()
    .HasMany(t => t.Equipment)
    .WithOne()
    .HasForeignKey(e => e.TourId)
    .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Tour>()
    .HasMany(t => t.KeyPoints)
    .WithOne()
    .HasForeignKey(kp => kp.TourId)
    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tour>()
    .Property(t => t.Duration)
    .HasConversion(
        v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
        v => JsonSerializer.Deserialize<List<TourDuration>>(v, new JsonSerializerOptions())!
    )
    .HasColumnType("jsonb");


    }

    private static void ConfigureTouristEquipment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TouristEquipment>(b =>
        {
            b.HasKey(te => te.Id);
            b.HasIndex(te => te.PersonId);
            b.HasIndex(te => new { te.PersonId, te.EquipmentId }).IsUnique();
        });
    }
}
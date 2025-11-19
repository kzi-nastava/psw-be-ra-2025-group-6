using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<TouristEquipment> TouristEquipment { get; set; }
    public DbSet<Tour> Tours { get; set; }

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        ConfigureTouristEquipment(modelBuilder);
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
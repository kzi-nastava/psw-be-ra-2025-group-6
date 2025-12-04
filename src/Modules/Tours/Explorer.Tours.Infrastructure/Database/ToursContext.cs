using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;

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
    public DbSet<KeyPoint> KeyPoints { get; set; }



    public DbSet<Facility> Facility { get; set; }

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        ConfigureTouristEquipment(modelBuilder);
        ConfigureKeyPoints(modelBuilder);
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

    private static void ConfigureKeyPoints(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KeyPoint>(b =>
        {
            b.HasKey(kp => kp.Id);
            b.HasIndex(kp => kp.TourId);
            b.HasOne<Tour>()
                .WithMany(t => t.KeyPoints)
                .HasForeignKey(kp => kp.TourId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

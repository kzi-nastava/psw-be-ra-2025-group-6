using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Explorer.Tours.Infrastructure.Database.Entities;

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



    public DbSet<Facility> Facility { get; set; }

    public DbSet<TourExecutionEntity> TourExecutions { get; set; }

    public DbSet<KeyPoint> KeyPoints { get; set; }

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        ConfigureTouristEquipment(modelBuilder);

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


        modelBuilder.Entity<TourExecutionEntity>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.InitialPositionJson).HasColumnType("jsonb");
            b.Property(e => e.ExecutionKeyPointsJson).HasColumnType("jsonb");
            b.Property(e => e.CompletedKeyPointsJson).HasColumnType("jsonb");
            b.Property(e => e.ProgressPercentage).HasDefaultValue(0);
        });
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
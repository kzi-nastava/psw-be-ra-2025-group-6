using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<ReviewApp> ReviewApps { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<TouristPosition> TouristPositions { get; set; }

    public DbSet<TourProblem> TourProblems { get; set; }


    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        
        modelBuilder.Entity<TouristPosition>().HasIndex(tp => tp.TouristId).IsUnique();

        modelBuilder.Entity<User>()
       .Property(u => u.Role)
       .HasConversion<string>();

        ConfigureStakeholder(modelBuilder);
        
        modelBuilder.Entity<UserProfile>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<UserProfile>(s => s.UserId);
        ConfigureReview(modelBuilder);
    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);
    }

    private static void ConfigureReview(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReviewApp>(builder =>
        {
            builder.ToTable("ReviewApp");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.UserId).IsRequired();
            builder.Property(r => r.Rating).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(500);
            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.UpdatedAt);
            builder.HasIndex(r => r.UserId).IsUnique();
            builder
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<ReviewApp>(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

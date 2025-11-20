using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Profile> Profiles { get; set; }

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        ConfigureStakeholder(modelBuilder);
    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);

        modelBuilder.Entity<Profile>()
            .HasIndex(profile => profile.PersonId)
            .IsUnique();

        modelBuilder.Entity<Profile>()
            .HasOne<Person>()
            .WithOne()
            .HasForeignKey<Profile>(profile => profile.PersonId);

        modelBuilder.Entity<Profile>()
            .Property(profile => profile.Biography)
            .HasMaxLength(250);

        modelBuilder.Entity<Profile>()
            .Property(profile => profile.Motto)
            .HasMaxLength(250);
    }
}
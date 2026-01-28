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
    public DbSet<Follow> Following { get; set; }
    public DbSet<TouristPosition> TouristPositions { get; set; }
    public DbSet<TourProblem> TourProblems { get; set; }
    public DbSet<TourProblemMessage> TourProblemMessages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<ProfilePost> ProfilePosts { get; set; }
    public DbSet<ClubPost> ClubPosts { get; set; }
    public DbSet<SocialMessage> SocialMessages { get; set; }

    public DbSet<Achievement> Achievement { get; set; }

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<Achievement>().HasIndex(u => u.Code).IsUnique();

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
        ConfigureTourProblemMessage(modelBuilder);
        ConfigureNotification(modelBuilder);
        ConfigureTourProblems(modelBuilder);
        ConfigureProfilePosts(modelBuilder);
        ConfigureClubPosts(modelBuilder);
        ConfigureFollowing(modelBuilder);
        ConfigureSocialMessages(modelBuilder);

        modelBuilder.Entity<UserProfile>()
    .HasMany(up => up.Achievements)
    .WithMany(a => a.UserProfiles)
    .UsingEntity(j =>
    {
        j.ToTable("UserAchievements");
    });

    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);
    }

    private static void ConfigureTourProblems(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TourProblem>(builder =>
        {
            builder.Property(p => p.Status).IsRequired();
            builder.Property(p => p.ReportedAt).IsRequired();
            builder.Property(p => p.DeadlineAt).IsRequired(false);
            builder.Property(p => p.ResolvedAt).IsRequired(false);
            builder.Property(p => p.ResolutionFeedback).IsRequired();
            builder.Property(p => p.ResolutionComment).HasMaxLength(1000).IsRequired(false);
            builder.Property(p => p.ResolutionAt).IsRequired(false);
        });
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

    private static void ConfigureTourProblemMessage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TourProblemMessage>()
            .HasOne<TourProblem>()
            .WithMany()
            .HasForeignKey(s => s.TourProblemId);
    }

    private static void ConfigureNotification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>()
            .Property(n => n.Status)
            .HasConversion<string>();
    }

    private static void ConfigureProfilePosts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProfilePost>(builder =>
        {
            builder.ToTable("ProfilePosts");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.AuthorId).IsRequired();
            builder.Property(p => p.Text).IsRequired().HasMaxLength(280);
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.UpdatedAt).IsRequired();
            builder.Property(p => p.ResourceType)
                .HasConversion<string?>()
                .HasMaxLength(20);
            builder.HasIndex(p => p.AuthorId);
        });
    }

    private static void ConfigureClubPosts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClubPost>(builder =>
        {
            builder.ToTable("ClubPosts");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.AuthorId).IsRequired();
            builder.Property(p => p.ClubId).IsRequired();
            builder.Property(p => p.Text).IsRequired().HasMaxLength(280);
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.UpdatedAt);
            builder.Property(p => p.ResourceType)
                .HasConversion<string?>()
                .HasMaxLength(20);
            builder.HasIndex(p => p.ClubId);
        });
    }

    private static void ConfigureFollowing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Follow>(builder =>
        {
            builder.ToTable("Following");

            builder.HasKey(f => f.Id);

            builder.HasIndex(f => new { f.FollowerId, f.FollowedId })
                   .IsUnique();

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(f => f.FollowerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(f => f.FollowedId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSocialMessages(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SocialMessage>(builder =>
        {
            builder.ToTable("SocialMessages");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.SenderId).IsRequired();
            builder.Property(m => m.ReceiverId).IsRequired();
            builder.Property(m => m.Content).IsRequired().HasMaxLength(2000);
            builder.Property(m => m.Timestamp).IsRequired();

            builder.HasIndex(m => new { m.SenderId, m.ReceiverId });
        });
    }
}

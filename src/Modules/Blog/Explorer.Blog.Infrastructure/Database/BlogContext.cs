using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public DbSet<BlogPost> Blogs { get; set; }
    public DbSet<BlogLocation> BlogLocations { get; set; }

    public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        ConfigureBlogPost(modelBuilder);

        modelBuilder.Entity<BlogPost>(b =>
        {
            b.HasKey(x => x.Id);

            b.OwnsMany(x => x.Votes, v =>
            {
                v.WithOwner().HasForeignKey("BlogId");
                v.Property(vote => vote.Type).HasColumnName("Value");
                v.Property(vote => vote.VotedAt);
                v.Property(vote => vote.UserId);
                v.HasKey("BlogId", "UserId");
                v.ToTable("Votes", "blog");
            });

            b.HasOne(x => x.Location)
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureBlogPost(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlogPost>(builder =>
        {
            builder.ToTable("Blogs", "blog");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Images)
               .HasColumnType("text[]");

            builder.OwnsMany(b => b.Comments, comments =>
            {
                comments.ToJson();            
                comments.Property(c => c.UserId);
                comments.Property(c => c.AuthorName);
                comments.Property(c => c.Text);
                comments.Property(c => c.CreatedAt);
                comments.Property(c => c.LastUpdatedAt);
            });

            builder.OwnsMany(b => b.ContentItems, content =>
            {
                content.Property(c => c.Order).IsRequired();
                content.Property(c => c.Type).IsRequired();
                content.Property(c => c.Content).IsRequired();
                content.ToTable("BlogContentItems", "blog");
            });
        });
    }

}
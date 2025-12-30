using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public DbSet<BlogPost> Blogs { get; set; }

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

            builder.OwnsMany(b => b.Comments, cb =>
            {
                cb.WithOwner().HasForeignKey("BlogId");

                cb.HasKey("BlogId", "Id");

                cb.Property(c => c.UserId);
                cb.Property(c => c.AuthorName);
                cb.Property(c => c.AuthorProfilePicture);
                cb.Property(c => c.Text);
                cb.Property(c => c.CreatedAt);
                cb.Property(c => c.LastUpdatedAt);

                cb.ToTable("Comments", "blog");
            });
        });
    }

}
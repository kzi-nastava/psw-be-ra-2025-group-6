using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public DbSet<BlogPost> Blogs { get; set; }

    public BlogContext(DbContextOptions<BlogContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        ConfigureBlogPost(modelBuilder);
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
        });
    }

}
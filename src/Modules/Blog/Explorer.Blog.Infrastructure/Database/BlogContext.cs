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
        modelBuilder.Entity<BlogPost>().Property(comment => comment.Comments).HasColumnType("jsonb");
    }
}
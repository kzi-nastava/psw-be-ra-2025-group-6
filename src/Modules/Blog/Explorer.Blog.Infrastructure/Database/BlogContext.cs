using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public DbSet<BlogPost> Blogs { get; set; }

    public BlogContext(DbContextOptions<BlogContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

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
}
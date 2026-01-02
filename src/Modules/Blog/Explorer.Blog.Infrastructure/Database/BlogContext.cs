using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public DbSet<BlogPost> Blogs { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<CommentReport> CommentReports { get; set; }

    public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        ConfigureBlogPost(modelBuilder);
        ConfigureComments(modelBuilder);
        ConfigureCommentLikes(modelBuilder);
        ConfigureCommentReports(modelBuilder);

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

            builder.HasMany(b => b.Comments)
                .WithOne()
                .HasForeignKey(c => c.BlogId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureComments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(cb =>
        {
            cb.ToTable("Comments", "blog");
            cb.HasKey(c => c.Id);

            cb.Property(c => c.UserId).IsRequired();
            cb.Property(c => c.AuthorName).IsRequired();
            cb.Property(c => c.AuthorProfilePicture).IsRequired();
            cb.Property(c => c.Text).IsRequired();
            cb.Property(c => c.CreatedAt).IsRequired();
            cb.Property(c => c.LastUpdatedAt);

            cb.HasIndex(c => c.BlogId);
        });
    }

    private static void ConfigureCommentLikes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommentLike>(b =>
        {
            b.ToTable("CommentLikes", "blog");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.BlogId, x.CommentId, x.UserId }).IsUnique();

            b.HasOne<BlogPost>()
                .WithMany()
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureCommentReports(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommentReport>(b =>
        {
            b.ToTable("CommentReports", "blog");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.BlogId, x.CommentId, x.UserId }).IsUnique();

            b.HasOne<BlogPost>()
                .WithMany()
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
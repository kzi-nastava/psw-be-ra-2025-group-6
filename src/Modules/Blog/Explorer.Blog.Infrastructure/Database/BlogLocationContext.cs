using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database
{
    public class BlogLocationContext : DbContext
    {
        public DbSet<BlogLocation> BlogLocations { get; set; }

        public BlogLocationContext(DbContextOptions<BlogLocationContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("location");

            ConfigureBlogLocation(modelBuilder);
        }

        private static void ConfigureBlogLocation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogLocation>(builder =>
            {
                builder.ToTable("BlogLocations", "location");

                builder.HasKey(l => l.Id);

                builder.HasIndex(l => new { l.Latitude, l.Longitude });
                builder.HasIndex(l => new { l.City, l.Country });

                builder.Property(l => l.City).HasMaxLength(200).IsRequired();
                builder.Property(l => l.Country).HasMaxLength(100).IsRequired();
                builder.Property(l => l.Region).HasMaxLength(100);
                builder.Property(l => l.Latitude).IsRequired();
                builder.Property(l => l.Longitude).IsRequired();
            });
        }
    }
}

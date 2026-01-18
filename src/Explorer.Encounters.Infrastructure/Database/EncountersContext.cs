using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Infrastructure.Database
{
    public class EncountersContext: DbContext
    {
        public EncountersContext(DbContextOptions<EncountersContext> options) : base(options) { }

        public DbSet<Challenge> Challenges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("encounters");

            modelBuilder.Entity<Challenge>(b =>
            {
                b.HasKey(c => c.Id);
                b.Property(c => c.Title).IsRequired();
                b.Property(c => c.Description).IsRequired();
                b.Property(c => c.Status).HasConversion<string>();
                b.Property(c => c.Type).HasConversion<string>();
            });
        }
    }
}

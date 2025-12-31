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
        public DbSet<TouristXpProfile> TouristXpProfiles { get; set; }
        public DbSet<EncounterCompletion> EncounterCompletions { get; set; }

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
                b.Property(c => c.CreatorId);
                b.Property(c => c.IsCreatedByTourist);
            });

            modelBuilder.Entity<TouristXpProfile>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.UserId).IsRequired();
                b.Property(p => p.CurrentXP).IsRequired();
                b.Property(p => p.Level).IsRequired();
                b.Property(p => p.LevelUpHistory).HasColumnType("jsonb");
                b.HasIndex(p => p.UserId).IsUnique();
            });

            modelBuilder.Entity<EncounterCompletion>(b =>
            {
                b.HasKey(c => c.Id);
                b.Property(c => c.UserId).IsRequired();
                b.Property(c => c.ChallengeId).IsRequired();
                b.Property(c => c.CompletedAt).IsRequired();
                b.Property(c => c.XpAwarded).IsRequired();
                b.HasIndex(c => new { c.UserId, c.ChallengeId }).IsUnique();
            });
        }
    }
}

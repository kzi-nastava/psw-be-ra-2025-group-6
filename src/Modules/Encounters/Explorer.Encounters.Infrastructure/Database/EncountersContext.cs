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

        public DbSet<SocialEncounter> SocialEncounters { get; set; }
        public DbSet<ActiveSocialParticipant> ActiveSocialParticipants { get; set; }

        public DbSet<HiddenLocationAttempt> HiddenLocationAttempts { get; set; }
        
        public DbSet<LeaderboardEntry> LeaderboardEntries { get; set; }
        public DbSet<ClubLeaderboard> ClubLeaderboards { get; set; }


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
                b.Property(c => c.ImagePath);
                b.Property(c => c.ActivationRadiusMeters).HasDefaultValue(50);
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


            // Konfiguriši SocialEncounter
            modelBuilder.Entity<SocialEncounter>().HasKey(x => x.Id);
            modelBuilder.Entity<SocialEncounter>()
                .HasIndex(x => x.ChallengeId)
                .IsUnique(); // Samo jedan SocialEncounter po Challenge-u

            // Konfiguriši ActiveSocialParticipant
            modelBuilder.Entity<ActiveSocialParticipant>().HasKey(x => x.Id);
            modelBuilder.Entity<ActiveSocialParticipant>()
                .HasIndex(x => new { x.UserId, x.SocialEncounterId })
                .IsUnique();

            modelBuilder.Entity<HiddenLocationAttempt>(b =>
            {
                b.HasKey(a => a.Id);
                b.Property(a => a.UserId).IsRequired();
                b.Property(a => a.ChallengeId).IsRequired();
                b.Property(a => a.StartedAt).IsRequired();
                b.Property(a => a.CompletedAt);
                b.Property(a => a.IsSuccessful).IsRequired();
                b.Property(a => a.SecondsInRadius).IsRequired();
                b.Property(a => a.LastPositionUpdate).IsRequired();
            });

            modelBuilder.Entity<LeaderboardEntry>(b =>
            {
                b.HasKey(l => l.Id);
                b.Property(l => l.UserId).IsRequired();
                b.Property(l => l.Username).IsRequired();
                b.Property(l => l.TotalXP).IsRequired();
                b.Property(l => l.CompletedChallenges).IsRequired();
                b.Property(l => l.CompletedTours).IsRequired();
                b.Property(l => l.AdventureCoins).IsRequired();
                b.Property(l => l.CurrentRank).IsRequired();
                b.Property(l => l.LastUpdated).IsRequired();
                b.Property(l => l.ClubId);
                b.HasIndex(l => l.UserId).IsUnique();
                b.HasIndex(l => l.CurrentRank);
            });

            modelBuilder.Entity<ClubLeaderboard>(b =>
            {
                b.HasKey(c => c.Id);
                b.Property(c => c.ClubId).IsRequired();
                b.Property(c => c.ClubName).IsRequired();
                b.Property(c => c.TotalXP).IsRequired();
                b.Property(c => c.TotalCompletedChallenges).IsRequired();
                b.Property(c => c.TotalCompletedTours).IsRequired();
                b.Property(c => c.TotalAdventureCoins).IsRequired();
                b.Property(c => c.MemberCount).IsRequired();
                b.Property(c => c.CurrentRank).IsRequired();
                b.Property(c => c.LastUpdated).IsRequired();
                b.HasIndex(c => c.ClubId).IsUnique();
                b.HasIndex(c => c.CurrentRank);
            });
        }
    }
}

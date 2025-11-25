using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Quiz;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<QuizQuestion> QuizQuestions { get; set; }
    public DbSet<QuizAnswerOption> QuizAnswerOptions { get; set; }

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        modelBuilder.Entity<Quiz>()
            .HasMany(q => q.Questions)
            .WithOne()
            .HasForeignKey(q => q.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuizQuestion>()
            .HasMany(q => q.Options)
            .WithOne()
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuizAnswerOption>()
            .Property(o => o.Text)
            .IsRequired();
        modelBuilder.Entity<QuizAnswerOption>()
            .Property(o => o.Feedback)
            .IsRequired();
        modelBuilder.Entity<QuizQuestion>()
            .Property(q => q.Text)
            .IsRequired();
        modelBuilder.Entity<Quiz>()
            .Property(q => q.Title)
            .IsRequired();
    }
}

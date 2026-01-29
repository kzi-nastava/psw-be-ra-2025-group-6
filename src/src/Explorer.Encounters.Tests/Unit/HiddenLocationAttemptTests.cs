using Explorer.Encounters.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Encounters.Tests.Unit;

public class HiddenLocationAttemptTests
{
    [Fact]
    public void Creates_attempt_with_valid_data()
    {
        // Arrange & Act
        var attempt = new HiddenLocationAttempt(1, 10);

        // Assert
        attempt.UserId.ShouldBe(1);
        attempt.ChallengeId.ShouldBe(10);
        attempt.IsSuccessful.ShouldBeFalse();
        attempt.SecondsInRadius.ShouldBe(0);
        attempt.CompletedAt.ShouldBeNull();
    }

    [Fact]
    public void Create_fails_with_invalid_user_id()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => new HiddenLocationAttempt(0, 10));
        Should.Throw<ArgumentException>(() => new HiddenLocationAttempt(-5, 10));
    }

    [Fact]
    public void Create_fails_with_invalid_challenge_id()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => new HiddenLocationAttempt(1, 0));
        Should.Throw<ArgumentException>(() => new HiddenLocationAttempt(1, -5));
    }

    [Fact]
    public void UpdateProgress_increments_seconds_when_in_radius()
    {
        // Arrange
        var attempt = new HiddenLocationAttempt(1, 10);
        System.Threading.Thread.Sleep(2000); // Wait 2 seconds

        // Act
        attempt.UpdateProgress(isInRadius: true);

        // Assert
        attempt.SecondsInRadius.ShouldBeGreaterThan(0);
        attempt.IsSuccessful.ShouldBeFalse(); // Not yet 30 seconds
    }

    [Fact]
    public void UpdateProgress_resets_timer_when_leaving_radius()
    {
        // Arrange
        var attempt = new HiddenLocationAttempt(1, 10);
        System.Threading.Thread.Sleep(2000);
        attempt.UpdateProgress(isInRadius: true);
        var secondsAfterFirst = attempt.SecondsInRadius;

        System.Threading.Thread.Sleep(1000);

        // Act - Leave radius
        attempt.UpdateProgress(isInRadius: false);

        // Assert
        attempt.SecondsInRadius.ShouldBe(0);
        attempt.IsSuccessful.ShouldBeFalse();
    }

    [Fact]
    public void UpdateProgress_marks_successful_after_30_seconds()
    {
        // Arrange
        var attempt = new HiddenLocationAttempt(1, 10);

        // Simulate 30+ seconds in radius
        for (int i = 0; i < 6; i++)
        {
            System.Threading.Thread.Sleep(5000);
            attempt.UpdateProgress(isInRadius: true);
        }

        // Assert
        attempt.IsSuccessful.ShouldBeTrue();
        attempt.SecondsInRadius.ShouldBeGreaterThanOrEqualTo(30);
        attempt.CompletedAt.ShouldBeNull();
    }

    [Fact]
    public void Complete_succeeds_when_attempt_is_successful()
    {
        // Arrange
        var attempt = new HiddenLocationAttempt(1, 10);
        
        // Simulate completion
        for (int i = 0; i < 6; i++)
        {
            System.Threading.Thread.Sleep(5000);
            attempt.UpdateProgress(isInRadius: true);
        }

        var completedAtBeforeComplete = attempt.CompletedAt;

        // Act
        attempt.Complete();

        // Assert
        completedAtBeforeComplete.ShouldBeNull();
        attempt.CompletedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Complete_fails_when_attempt_not_successful()
    {
        // Arrange
        var attempt = new HiddenLocationAttempt(1, 10);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => attempt.Complete());
    }

    [Fact]
    public void CanComplete_returns_true_when_successful()
    {
        // Arrange
        var attempt = new HiddenLocationAttempt(1, 10);
        
        for (int i = 0; i < 6; i++)
        {
            System.Threading.Thread.Sleep(5000);
            attempt.UpdateProgress(isInRadius: true);
        }

        // Act & Assert
        attempt.CanComplete().ShouldBeTrue();
    }

    [Fact]
    public void CanComplete_returns_false_when_not_successful()
    {
        // Arrange
        var attempt = new HiddenLocationAttempt(1, 10);

        // Act & Assert
        attempt.CanComplete().ShouldBeFalse();
    }
}

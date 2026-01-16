using AvailabilityCompass.Core.Features.ManageCalendars;
using Shouldly;

namespace AvailabilityCompass.Core.Tests.Features.ManageCalendars;

public class DetectedSelectionTests
{
    [Fact]
    public void IsPeriod_ShouldReturnFalse_WhenDurationIsOne()
    {
        // Arrange
        var selection = new DetectedSelection(new DateOnly(2025, 1, 15), 1);

        // Act & Assert
        selection.IsPeriod.ShouldBeFalse();
    }

    [Fact]
    public void IsPeriod_ShouldReturnTrue_WhenDurationIsGreaterThanOne()
    {
        // Arrange
        var selection = new DetectedSelection(new DateOnly(2025, 1, 15), 2);

        // Act & Assert
        selection.IsPeriod.ShouldBeTrue();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(365)]
    public void IsPeriod_ShouldReturnTrue_ForVariousDurations(int duration)
    {
        // Arrange
        var selection = new DetectedSelection(new DateOnly(2025, 1, 15), duration);

        // Act & Assert
        selection.IsPeriod.ShouldBeTrue();
    }

    [Fact]
    public void DisplayText_ShouldShowSingleDate_WhenDurationIsOne()
    {
        // Arrange
        var selection = new DetectedSelection(new DateOnly(2025, 1, 15), 1);

        // Act
        var displayText = selection.DisplayText;

        // Assert
        displayText.ShouldBe("2025-01-15");
    }

    [Fact]
    public void DisplayText_ShouldShowDateRange_WhenDurationIsGreaterThanOne()
    {
        // Arrange
        var selection = new DetectedSelection(new DateOnly(2025, 1, 15), 3);

        // Act
        var displayText = selection.DisplayText;

        // Assert
        displayText.ShouldBe("2025-01-15 to 2025-01-17 (3 days)");
    }

    [Fact]
    public void DisplayText_ShouldCalculateEndDateCorrectly_ForTwoDays()
    {
        // Arrange
        var selection = new DetectedSelection(new DateOnly(2025, 1, 15), 2);

        // Act
        var displayText = selection.DisplayText;

        // Assert
        displayText.ShouldBe("2025-01-15 to 2025-01-16 (2 days)");
    }

    [Fact]
    public void DisplayText_ShouldHandleMonthBoundary()
    {
        // Arrange - starts Jan 30, duration 5 days crosses into February
        var selection = new DetectedSelection(new DateOnly(2025, 1, 30), 5);

        // Act
        var displayText = selection.DisplayText;

        // Assert
        displayText.ShouldBe("2025-01-30 to 2025-02-03 (5 days)");
    }

    [Fact]
    public void DisplayText_ShouldHandleYearBoundary()
    {
        // Arrange - starts Dec 30, duration 5 days crosses into next year
        var selection = new DetectedSelection(new DateOnly(2024, 12, 30), 5);

        // Act
        var displayText = selection.DisplayText;

        // Assert
        displayText.ShouldBe("2024-12-30 to 2025-01-03 (5 days)");
    }

    [Fact]
    public void RecordEquality_ShouldWork()
    {
        // Arrange
        var selection1 = new DetectedSelection(new DateOnly(2025, 1, 15), 3);
        var selection2 = new DetectedSelection(new DateOnly(2025, 1, 15), 3);
        var selection3 = new DetectedSelection(new DateOnly(2025, 1, 15), 4);

        // Assert
        selection1.ShouldBe(selection2);
        selection1.ShouldNotBe(selection3);
    }
}
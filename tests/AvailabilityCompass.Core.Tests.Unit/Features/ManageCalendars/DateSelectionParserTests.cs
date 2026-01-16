using AvailabilityCompass.Core.Features.ManageCalendars;
using Shouldly;

namespace AvailabilityCompass.Core.Tests.Features.ManageCalendars;

public class DateSelectionParserTests
{
    private readonly DateSelectionParser _sut = new();

    [Fact]
    public void ParseSelectedDates_ShouldReturnEmptyList_WhenInputIsEmpty()
    {
        // Arrange
        var dates = Array.Empty<DateTime>();

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ParseSelectedDates_ShouldReturnSingleSelection_WhenSingleDateProvided()
    {
        // Arrange
        var dates = new[] { new DateTime(2025, 1, 15) };

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.ShouldHaveSingleItem();
        result[0].StartDate.ShouldBe(new DateOnly(2025, 1, 15));
        result[0].Duration.ShouldBe(1);
        result[0].IsPeriod.ShouldBeFalse();
    }

    [Fact]
    public void ParseSelectedDates_ShouldReturnSinglePeriod_WhenTwoConsecutiveDatesProvided()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2025, 1, 15),
            new DateTime(2025, 1, 16)
        };

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.ShouldHaveSingleItem();
        result[0].StartDate.ShouldBe(new DateOnly(2025, 1, 15));
        result[0].Duration.ShouldBe(2);
        result[0].IsPeriod.ShouldBeTrue();
    }

    [Fact]
    public void ParseSelectedDates_ShouldReturnSinglePeriod_WhenMultipleConsecutiveDatesProvided()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2025, 1, 15),
            new DateTime(2025, 1, 16),
            new DateTime(2025, 1, 17),
            new DateTime(2025, 1, 18),
            new DateTime(2025, 1, 19)
        };

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.ShouldHaveSingleItem();
        result[0].StartDate.ShouldBe(new DateOnly(2025, 1, 15));
        result[0].Duration.ShouldBe(5);
        result[0].IsPeriod.ShouldBeTrue();
    }

    [Fact]
    public void ParseSelectedDates_ShouldReturnMultipleSelections_WhenGapBetweenDates()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2025, 1, 15),
            new DateTime(2025, 1, 17) // Gap on 16th
        };

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.Count.ShouldBe(2);
        result[0].StartDate.ShouldBe(new DateOnly(2025, 1, 15));
        result[0].Duration.ShouldBe(1);
        result[1].StartDate.ShouldBe(new DateOnly(2025, 1, 17));
        result[1].Duration.ShouldBe(1);
    }

    [Fact]
    public void ParseSelectedDates_ShouldDetectMultipleDisjointPeriods()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2025, 1, 15),
            new DateTime(2025, 1, 16),
            new DateTime(2025, 1, 17),
            // Gap
            new DateTime(2025, 1, 20),
            new DateTime(2025, 1, 21),
            // Gap
            new DateTime(2025, 1, 25)
        };

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.Count.ShouldBe(3);

        result[0].StartDate.ShouldBe(new DateOnly(2025, 1, 15));
        result[0].Duration.ShouldBe(3);
        result[0].IsPeriod.ShouldBeTrue();

        result[1].StartDate.ShouldBe(new DateOnly(2025, 1, 20));
        result[1].Duration.ShouldBe(2);
        result[1].IsPeriod.ShouldBeTrue();

        result[2].StartDate.ShouldBe(new DateOnly(2025, 1, 25));
        result[2].Duration.ShouldBe(1);
        result[2].IsPeriod.ShouldBeFalse();
    }

    [Fact]
    public void ParseSelectedDates_ShouldSortDatesCorrectly_WhenProvidedOutOfOrder()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2025, 1, 17),
            new DateTime(2025, 1, 15),
            new DateTime(2025, 1, 16)
        };

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.ShouldHaveSingleItem();
        result[0].StartDate.ShouldBe(new DateOnly(2025, 1, 15));
        result[0].Duration.ShouldBe(3);
    }

    [Fact]
    public void ParseSelectedDates_ShouldHandleLargeGaps()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31) // Nearly a year gap
        };

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.Count.ShouldBe(2);
        result[0].StartDate.ShouldBe(new DateOnly(2025, 1, 1));
        result[0].Duration.ShouldBe(1);
        result[1].StartDate.ShouldBe(new DateOnly(2025, 12, 31));
        result[1].Duration.ShouldBe(1);
    }

    [Fact]
    public void ParseSelectedDates_ShouldHandleYearBoundary()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2024, 12, 31),
            new DateTime(2025, 1, 1),
            new DateTime(2025, 1, 2)
        };

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.ShouldHaveSingleItem();
        result[0].StartDate.ShouldBe(new DateOnly(2024, 12, 31));
        result[0].Duration.ShouldBe(3);
    }

    [Fact]
    public void ParseSelectedDates_ShouldHandleMonthBoundary()
    {
        // Arrange
        var dates = new[]
        {
            new DateTime(2025, 1, 30),
            new DateTime(2025, 1, 31),
            new DateTime(2025, 2, 1),
            new DateTime(2025, 2, 2)
        };

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.ShouldHaveSingleItem();
        result[0].StartDate.ShouldBe(new DateOnly(2025, 1, 30));
        result[0].Duration.ShouldBe(4);
    }

    [Fact]
    public void ParseSelectedDates_ShouldHandleLeapYear()
    {
        // Arrange - 2024 is a leap year
        var dates = new[]
        {
            new DateTime(2024, 2, 28),
            new DateTime(2024, 2, 29),
            new DateTime(2024, 3, 1)
        };

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.ShouldHaveSingleItem();
        result[0].StartDate.ShouldBe(new DateOnly(2024, 2, 28));
        result[0].Duration.ShouldBe(3);
    }

    [Fact]
    public void ParseSelectedDates_ShouldHandleNonLeapYear()
    {
        // Arrange - 2025 is not a leap year
        var dates = new[]
        {
            new DateTime(2025, 2, 28),
            new DateTime(2025, 3, 1)
        };

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.ShouldHaveSingleItem();
        result[0].StartDate.ShouldBe(new DateOnly(2025, 2, 28));
        result[0].Duration.ShouldBe(2);
    }

    [Fact]
    public void ParseSelectedDates_ShouldIgnoreTimeComponent()
    {
        // Arrange - Same date with different times should be treated as one date
        var dates = new[]
        {
            new DateTime(2025, 1, 15, 8, 0, 0),
            new DateTime(2025, 1, 16, 14, 30, 0),
            new DateTime(2025, 1, 17, 23, 59, 59)
        };

        // Act
        var result = _sut.ParseSelectedDates(dates);

        // Assert
        result.ShouldHaveSingleItem();
        result[0].Duration.ShouldBe(3);
    }
}
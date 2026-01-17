using AvailabilityCompass.Core.Features.ManageSources.Sources;
using Shouldly;

namespace AvailabilityCompass.Core.Tests.Features.ManageSources.Sources;

public class PolishDateParserTests
{
    [Theory]
    [InlineData("stycznia", 1)]
    [InlineData("lutego", 2)]
    [InlineData("marca", 3)]
    [InlineData("kwietnia", 4)]
    [InlineData("maja", 5)]
    [InlineData("czerwca", 6)]
    [InlineData("lipca", 7)]
    [InlineData("sierpnia", 8)]
    [InlineData("września", 9)]
    [InlineData("października", 10)]
    [InlineData("listopada", 11)]
    [InlineData("grudnia", 12)]
    public void GetMonthNumber_ShouldReturnCorrectMonth_ForGenitivePolishMonthNames(string monthName, int expected)
    {
        // Act
        var result = PolishDateParser.GetMonthNumber(monthName);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("styczeń", 1)]
    [InlineData("luty", 2)]
    [InlineData("marzec", 3)]
    [InlineData("kwiecień", 4)]
    [InlineData("maj", 5)]
    [InlineData("czerwiec", 6)]
    [InlineData("lipiec", 7)]
    [InlineData("sierpień", 8)]
    [InlineData("wrzesień", 9)]
    [InlineData("październik", 10)]
    [InlineData("listopad", 11)]
    [InlineData("grudzień", 12)]
    public void GetMonthNumber_ShouldReturnCorrectMonth_ForNominativePolishMonthNames(string monthName, int expected)
    {
        // Act
        var result = PolishDateParser.GetMonthNumber(monthName);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("MAJA", 5)]
    [InlineData("Maja", 5)]
    [InlineData("STYCZNIA", 1)]
    [InlineData("Grudnia", 12)]
    public void GetMonthNumber_ShouldBeCaseInsensitive(string monthName, int expected)
    {
        // Act
        var result = PolishDateParser.GetMonthNumber(monthName);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("january")]
    [InlineData("")]
    [InlineData("abc")]
    public void GetMonthNumber_ShouldReturnCurrentMonth_ForInvalidMonthName(string monthName)
    {
        // Act
        var result = PolishDateParser.GetMonthNumber(monthName);

        // Assert
        result.ShouldBe(DateTime.Now.Month);
    }

    [Theory]
    [InlineData("15 maja 2025", 2025, 5, 15)]
    [InlineData("1 stycznia 2024", 2024, 1, 1)]
    [InlineData("31 grudnia 2023", 2023, 12, 31)]
    [InlineData("28 lutego 2024", 2024, 2, 28)]
    [InlineData("10 października 2025", 2025, 10, 10)]
    public void TryParsePolishDate_ShouldParseFullDayMonthYear_Correctly(
        string input,
        int expectedYear,
        int expectedMonth,
        int expectedDay)
    {
        // Act
        var success = PolishDateParser.TryParsePolishDate(input, out var result);

        // Assert
        success.ShouldBeTrue();
        result.Year.ShouldBe(expectedYear);
        result.Month.ShouldBe(expectedMonth);
        result.Day.ShouldBe(expectedDay);
    }

    [Theory]
    [InlineData("15 maja")]
    [InlineData("1 stycznia")]
    [InlineData("28 lutego")]
    public void TryParsePolishDate_ShouldUseCurrentYear_WhenYearNotProvided(string input)
    {
        // Act
        var success = PolishDateParser.TryParsePolishDate(input, out var result);

        // Assert
        success.ShouldBeTrue();
        result.Year.ShouldBe(DateTime.Now.Year);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("maja 15")] // Wrong order
    [InlineData("abc def")]
    public void TryParsePolishDate_ShouldReturnFalse_ForInvalidInput(string? input)
    {
        // Act
        var success = PolishDateParser.TryParsePolishDate(input, out var result);

        // Assert
        success.ShouldBeFalse();
        result.ShouldBe(DateOnly.MinValue);
    }

    [Theory]
    [InlineData("32 maja 2025")] // Invalid day (too high)
    [InlineData("0 maja 2025")] // Invalid day (zero)
    [InlineData("-1 maja 2025")] // Invalid day (negative)
    public void TryParsePolishDate_ShouldReturnFalse_ForInvalidDayValues(string input)
    {
        // Act
        var success = PolishDateParser.TryParsePolishDate(input, out _);

        // Assert
        success.ShouldBeFalse();
    }

    [Fact]
    public void TryParsePolishDate_ShouldReturnFalse_ForInvalidDateCombination()
    {
        // Arrange - February 30th doesn't exist
        var input = "30 lutego 2025";

        // Act
        var success = PolishDateParser.TryParsePolishDate(input, out _);

        // Assert
        success.ShouldBeFalse();
    }

    [Fact]
    public void TryParsePolishDate_ShouldHandleLeapYear_Correctly()
    {
        // Arrange - February 29th in a leap year
        var input = "29 lutego 2024"; // 2024 is a leap year

        // Act
        var success = PolishDateParser.TryParsePolishDate(input, out var result);

        // Assert
        success.ShouldBeTrue();
        result.ShouldBe(new DateOnly(2024, 2, 29));
    }

    [Fact]
    public void TryParsePolishDate_ShouldReturnFalse_ForFebruary29InNonLeapYear()
    {
        // Arrange - February 29th in a non-leap year
        var input = "29 lutego 2025"; // 2025 is not a leap year

        // Act
        var success = PolishDateParser.TryParsePolishDate(input, out _);

        // Assert
        success.ShouldBeFalse();
    }

    [Theory]
    [InlineData("15   maja   2025")] // Extra spaces
    [InlineData("  15 maja 2025  ")] // Leading/trailing spaces get trimmed by Split
    public void TryParsePolishDate_ShouldHandleExtraSpaces(string input)
    {
        // Act
        var success = PolishDateParser.TryParsePolishDate(input, out var result);

        // Assert
        success.ShouldBeTrue();
        result.ShouldBe(new DateOnly(2025, 5, 15));
    }
}
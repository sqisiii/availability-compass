using System.Collections;
using AvailabilityCompass.Core.Features.SearchRecords;
using AvailabilityCompass.Core.Shared;
using NSubstitute;
using Shouldly;

namespace AvailabilityCompass.Core.Tests.Features.SearchRecords.Search;

public class SourceFilterViewModelTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    public SourceFilterViewModelTests()
    {
    }

    [Fact]
    public void LastUpdated_ShouldReturnNAText_WhenChangeAtIsNull()
    {
        // Arrange && Act
        _dateTimeProvider.Now.Returns(new DateTime(2025, 04, 16, 12, 0, 0));
        var sut = new SourceFilterViewModel(_dateTimeProvider)
        {
            ChangeAt = null
        };

        // Assert
        sut.LastUpdated.ShouldBe("N/A");
    }

    [Theory]
    [InlineData("2025-04-16T12:00:00", "2025-04-16T12:00:00", "Just now")]
    [InlineData("2025-04-16T12:00:00", "2025-04-16T12:00:01", "Just now")]
    [InlineData("2025-04-16T12:00:00", "2025-04-16T12:00:59", "Just now")]
    public void LastUpdated_ShouldReturnJustNowText_WhenTimeDiffIsBelow1Minute(string changeAt, string now, string expected)
    {
        // Arrange && Act
        _dateTimeProvider.Now.Returns(DateTime.Parse(now));
        var sut = new SourceFilterViewModel(_dateTimeProvider)
        {
            ChangeAt = DateTime.Parse(changeAt)
        };

        // Assert
        sut.LastUpdated.ShouldBe(expected);
    }

    [Theory]
    [MemberData(nameof(GetMinutesAgoTestData))]
    public void LastUpdated_ShouldReturnMinutesAgoText_WhenTimeDiffIsBetween1And60Minutes(string changeAt, string now, string expected)
    {
        // Arrange && Act
        _dateTimeProvider.Now.Returns(DateTime.Parse(now));
        var sut = new SourceFilterViewModel(_dateTimeProvider)
        {
            ChangeAt = DateTime.Parse(changeAt)
        };

        // Assert
        sut.LastUpdated.ShouldBe(expected);
    }

    public static IEnumerable<object[]> GetMinutesAgoTestData() =>
        new List<object[]>
        {
            new object[] { "2025-04-16T12:00:00", "2025-04-16T12:01:00", "1 minute ago" },
            new object[] { "2025-04-16T12:00:00", "2025-04-16T12:01:01", "1 minute ago" },
            new object[] { "2025-04-16T12:00:00", "2025-04-16T12:02:00", "2 minutes ago" },
            new object[] { "2025-04-16T12:00:00", "2025-04-16T12:59:59", "59 minutes ago" }
        };

    [Theory]
    [ClassData(typeof(GetHoursAgoTestData))]
    public void LastUpdated_ShouldReturnHoursAgoText_WhenTimeDiffIsBetween1And24Hours(string changeAt, string now, string expected)
    {
        // Arrange && Act
        _dateTimeProvider.Now.Returns(DateTime.Parse(now));
        var sut = new SourceFilterViewModel(_dateTimeProvider)
        {
            ChangeAt = DateTime.Parse(changeAt)
        };

        // Assert
        sut.LastUpdated.ShouldBe(expected);
    }

    [Theory]
    [MemberData(nameof(GetDaysAgoTestData))]
    public void LastUpdated_ShouldReturnDaysAgoText_WhenTimeDiffIsOver1Day(string changeAt, string now, string expected)
    {
        // Arrange && Act
        _dateTimeProvider.Now.Returns(DateTime.Parse(now));
        var sut = new SourceFilterViewModel(_dateTimeProvider)
        {
            ChangeAt = DateTime.Parse(changeAt)
        };

        // Assert
        sut.LastUpdated.ShouldBe(expected);
    }

    public static IEnumerable<object[]> GetDaysAgoTestData() =>
        new List<object[]>
        {
            new object[] { "2025-04-16T12:00:00", "2025-04-17T12:00:00", "1 day ago" },
            new object[] { "2025-04-16T12:00:00", "2025-04-18T12:00:00", "2 days ago" },
            new object[] { "2025-04-16T12:00:00", "2025-04-19T12:00:00", "3 days ago" },
            new object[] { "2025-04-16T12:00:00", "2025-05-16T12:00:00", "30 days ago" }
        };
}

public class GetHoursAgoTestData : IEnumerable<object[]>
{
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<object[]> GetEnumerator()
    {
        var baseTime = new DateTime(2025, 04, 16, 12, 0, 0);

        // Include exactly 1 hour difference
        var oneHourLaterTime = baseTime.AddHours(1);
        yield return [baseTime.ToString("yyyy-MM-ddTHH:mm:ss"), oneHourLaterTime.ToString("yyyy-MM-ddTHH:mm:ss"), "1 hour ago"];

        // Include a 23h 59m 59s difference
        var almostFullDayLaterTime = baseTime.AddHours(23).AddMinutes(59).AddSeconds(59);
        yield return [baseTime.ToString("yyyy-MM-ddTHH:mm:ss"), almostFullDayLaterTime.ToString("yyyy-MM-ddTHH:mm:ss"), "23 hours ago"];

        // Add 10 random cases between 1 and 24 hours
        var random = new Random(42);
        for (var i = 0; i < 5; i++)
        {
            var hours = random.Next(2, 25); // 2 to 24 inclusive
            var minutes = random.Next(60); // 0 to 59
            var seconds = random.Next(60); // 0 to 59

            var laterTime = baseTime.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            // For display, we only show the hour component in the expected result
            var expected = hours == 1 ? "1 hour ago" : $"{hours} hours ago";
            yield return [baseTime.ToString("yyyy-MM-ddTHH:mm:ss"), laterTime.ToString("yyyy-MM-ddTHH:mm:ss"), expected];
        }
    }
}
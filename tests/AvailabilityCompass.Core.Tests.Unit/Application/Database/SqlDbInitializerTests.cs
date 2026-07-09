using AvailabilityCompass.Core.Application.Database;
using AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendersForFilteringQuery;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetCalendars;
using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using Shouldly;

namespace AvailabilityCompass.Core.Tests.Application.Database;

public class SqlDbInitializerTests
{
    [Fact]
    public async Task InitializeAsync_ConvertsLegacyTextCalendarGuidsToBlobs_WhenCalendarRowsExist()
    {
        // Arrange
        var databasePath = GetTempDatabasePath();
        var calendarId = Guid.NewGuid();
        var dateEntryId = Guid.NewGuid();

        try
        {
            var factory = new SqliteDbConnectionFactory($"Data Source={databasePath};Pooling=False");
            await SeedLegacyCalendarDataAsync(factory, calendarId, dateEntryId);
            var sut = new SqlDbInitializer(factory);

            // Act
            await sut.InitializeAsync();

            // Assert
            using var connection = factory.Connect();
            var calendarIdType = await connection.QuerySingleAsync<string>(
                "SELECT typeof(CalendarId) FROM Calendar WHERE Name = @Name;",
                new { Name = "Legacy calendar" });
            var dateEntryTypes = await connection.QuerySingleAsync<(string CalendarIdType, string IdType)>(
                "SELECT typeof(CalendarId) AS CalendarIdType, typeof(Id) AS IdType FROM DateEntry WHERE Description = @Description;",
                new { Description = "Legacy entry" });

            calendarIdType.ShouldBe("blob");
            dateEntryTypes.CalendarIdType.ShouldBe("blob");
            dateEntryTypes.IdType.ShouldBe("blob");
        }
        finally
        {
            DeleteDatabaseFiles(databasePath);
        }
    }

    [Fact]
    public async Task GetCalendarsForFilteringHandler_ReturnsCalendars_WhenLegacyTextGuidsWereNormalized()
    {
        // Arrange
        var databasePath = GetTempDatabasePath();
        var calendarId = Guid.NewGuid();

        try
        {
            var factory = new SqliteDbConnectionFactory($"Data Source={databasePath};Pooling=False");
            await SeedLegacyCalendarDataAsync(factory, calendarId, Guid.NewGuid());
            var initializer = new SqlDbInitializer(factory);
            await initializer.InitializeAsync();
            var sut = new GetCalendarsForFilteringHandler(factory);

            // Act
            GetCalendarsForFilteringResponse result =
                await sut.Handle(new GetCalendarsForFilteringQuery(), CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Calendars.ShouldContain(calendar => calendar.Id == calendarId && calendar.Name == "Legacy calendar");
        }
        finally
        {
            DeleteDatabaseFiles(databasePath);
        }
    }

    private static string GetTempDatabasePath()
    {
        string directory = Path.Combine(Path.GetTempPath(), "availability-compass-tests");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, $"{Guid.NewGuid():N}.db");
    }

    private static async Task SeedLegacyCalendarDataAsync(
        IDbConnectionFactory factory,
        Guid calendarId,
        Guid dateEntryId)
    {
        using var connection = factory.Connect();
        const string createCalendarTableSql =
            """
            CREATE TABLE Calendar (
                CalendarId BLOB(16) NOT NULL,
                Name TEXT NOT NULL,
                IsOnly Integer NOT NULL,
                ChangeDate TEXT NOT NULL,
                PRIMARY KEY (CalendarId)
            );
            """;
        const string createDateEntryTableSql =
            """
            CREATE TABLE DateEntry (
                CalendarId BLOB(16) NOT NULL,
                Id BLOB(16) NOT NULL,
                StartDate TEXT NOT NULL,
                Description TEXT,
                IsRecurring INTEGER NOT NULL DEFAULT 0,
                Duration INTEGER NOT NULL DEFAULT 1,
                Frequency INTEGER,
                NumberOfRepetitions INTEGER NOT NULL DEFAULT 0,
                ChangeDate TEXT NOT NULL,
                PRIMARY KEY (CalendarId, Id),
                FOREIGN KEY (CalendarId) REFERENCES Calendar(CalendarId) ON DELETE CASCADE
            );
            """;

        await connection.ExecuteAsync(createCalendarTableSql);
        await connection.ExecuteAsync(createDateEntryTableSql);
        await connection.ExecuteAsync(
            """
            INSERT INTO Calendar (CalendarId, Name, IsOnly, ChangeDate)
            VALUES (@CalendarId, @Name, @IsOnly, @ChangeDate);
            """,
            new
            {
                CalendarId = calendarId.ToString(),
                Name = "Legacy calendar",
                IsOnly = true,
                ChangeDate = "2026-03-09T00:00:00Z"
            });
        await connection.ExecuteAsync(
            """
            INSERT INTO DateEntry (
                CalendarId,
                Id,
                StartDate,
                Description,
                IsRecurring,
                Duration,
                Frequency,
                NumberOfRepetitions,
                ChangeDate)
            VALUES (
                @CalendarId,
                @Id,
                @StartDate,
                @Description,
                @IsRecurring,
                @Duration,
                @Frequency,
                @NumberOfRepetitions,
                @ChangeDate);
            """,
            new
            {
                CalendarId = calendarId.ToString(),
                Id = dateEntryId.ToString(),
                StartDate = "2026-03-10",
                Description = "Legacy entry",
                IsRecurring = false,
                Duration = 1,
                Frequency = (int?)null,
                NumberOfRepetitions = 0,
                ChangeDate = "2026-03-09T00:00:00Z"
            });
    }

    private static void DeleteDatabaseFiles(string databasePath)
    {
        foreach (string path in new[] { databasePath, $"{databasePath}-wal", $"{databasePath}-shm" })
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}

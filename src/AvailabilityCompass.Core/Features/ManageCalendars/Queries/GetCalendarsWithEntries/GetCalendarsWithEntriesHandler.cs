using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendarsWithEntries;

/// <summary>
/// Handles retrieving calendars with their date entries by ids.
/// </summary>
public class GetCalendarsWithEntriesHandler : IRequestHandler<GetCalendarsWithEntriesQuery, GetCalendarsWithEntriesResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetCalendarsWithEntriesHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<GetCalendarsWithEntriesResponse> Handle(GetCalendarsWithEntriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            // language=SQLite
            const string sql = """
                               SELECT c.CalendarId, c.Name, c.IsOnly, c.ChangeDate,
                                   de.Id AS DateEntryId, de.CalendarId, de.Description, de.StartDate,
                                   de.IsRecurring, de.Duration, de.Frequency, de.NumberOfRepetitions, de.ChangeDate
                               FROM Calendar c
                               LEFT JOIN DateEntry de ON c.CalendarId = de.CalendarId
                               WHERE c.CalendarId IN @CalendarIds
                               ORDER BY c.Name, de.StartDate
                               """;

            var calendarDict = new Dictionary<Guid, CalendarDto>();

            await connection.QueryAsync<CalendarDto, DateEntryDto?, CalendarDto>(
                    sql,
                    map: (calendar, dateEntry) =>
                    {
                        if (!calendarDict.TryGetValue(calendar.CalendarId, out var calendarEntry))
                        {
                            calendarEntry = calendar;
                            calendarDict.Add(calendar.CalendarId, calendarEntry);
                        }

                        if (dateEntry is not null && calendarEntry.DateEntries.All(x => x.DateEntryId != dateEntry.DateEntryId))
                        {
                            calendarEntry.DateEntries.Add(dateEntry);
                        }

                        return calendarEntry;
                    },
                    param: new { request.CalendarIds },
                    splitOn: "DateEntryId")
                .ConfigureAwait(false);

            return new GetCalendarsWithEntriesResponse(true, calendarDict.Values.ToList());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting calendars with entries from database");
            return new GetCalendarsWithEntriesResponse(false, []);
        }
    }
}
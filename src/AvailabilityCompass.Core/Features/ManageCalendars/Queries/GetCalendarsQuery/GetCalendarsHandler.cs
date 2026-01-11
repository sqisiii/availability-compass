using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendarsQuery;

public class GetCalendarsHandler : IRequestHandler<GetCalendarsQuery, GetCalendarsResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetCalendarsHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<GetCalendarsResponse> Handle(GetCalendarsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            // language=SQLite
            const string sql = """
                               SELECT c.CalendarId, c.Name, c.IsOnly, c.ChangeDate,
                                    de.Id AS DateEntryId, de.CalendarId, de.StartDate, de.Description,
                                    de.IsRecurring, de.Duration, de.Frequency, de.NumberOfRepetitions, de.ChangeDate
                                FROM Calendar c
                                LEFT JOIN DateEntry de ON c.CalendarId = de.CalendarId
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
                    splitOn: "DateEntryId")
                .ConfigureAwait(false);

            return new GetCalendarsResponse(calendarDict.Values.ToList(), true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting calendars from database");
            return new GetCalendarsResponse(null, false);
        }
    }
}
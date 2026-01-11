using AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetAvailableDates;
using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetAvailableDates;

/// <summary>
/// Handles requests to get available dates for a set of calendars.
/// </summary>
/// <remarks>
/// This handler retrieves calendar data from the database and calculates reserved dates
/// based on date entries defined in the calendars.
/// Request and Response are defined in the different feature <see cref="GetAvailableDatesQuery"/> and <see cref="GetAvailableDatesResponse"/> classes.
/// </remarks>
public class GetAvailableDatesHandler : IRequestHandler<GetAvailableDatesQuery, GetAvailableDatesResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IReservedDatesCalculator _reservedDatesCalculator;

    public GetAvailableDatesHandler(
        IDbConnectionFactory dbConnectionFactory,
        IReservedDatesCalculator reservedDatesCalculator)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _reservedDatesCalculator = reservedDatesCalculator;
    }

    public async Task<GetAvailableDatesResponse> Handle(GetAvailableDatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var filteredCalendars = await GetCalendars(request.CalendarIds);

            var reservedDates = _reservedDatesCalculator.GetReservedDates(filteredCalendars);

            return new GetAvailableDatesResponse(true, reservedDates);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error calculating available dates: {ErrorMessage}", ex.Message);
            return new GetAvailableDatesResponse(false, []);
        }
    }

    private async Task<List<CalendarDto>> GetCalendars(List<Guid> calendarIds)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            // language=SQLite
            const string sql = """
                                SELECT c.CalendarId, c.Name, c.IsOnly, c.ChangeDate,
                                    de.Id as DateEntryId, de.CalendarId, de.Description, de.StartDate,
                                    de.IsRecurring, de.Duration, de.Frequency, de.NumberOfRepetitions, de.ChangeDate
                                FROM Calendar c
                                LEFT JOIN DateEntry de ON c.CalendarId = de.CalendarId
                                WHERE c.CalendarId IN @CalendarIds
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
                param: new { CalendarIds = calendarIds },
                splitOn: "DateEntryId");

            return calendarDict.Values.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting selected calendars from database");
            return [];
        }
    }
}
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
/// based on single and recurring dates defined in the calendars.
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

            const string sql = @"
                    SELECT c.CalendarId, c.Name, c.IsOnly, c.ChangeDate,
                        sd.Id as SingleDateId, sd.CalendarId, sd.Description as SingleDateDescription, sd.Date, sd.ChangeDate,
                        rd.Id as RecurringDateId, rd.CalendarId, rd.Description as RecurringDateDescription, rd.StartDate, rd.Duration, rd.Frequency, rd.NumberOfRepetitions, rd.ChangeDate
                    FROM Calendar c
                    LEFT JOIN SingleDate sd ON c.CalendarId = sd.CalendarId
                    LEFT JOIN RecurringDate rd ON c.CalendarId = rd.CalendarId
                    WHERE c.CalendarId IN @CalendarIds";

            var calendarDict = new Dictionary<Guid, CalendarDto>();

            await connection.QueryAsync<CalendarDto, SingleDateDto?, RecurringDateDto?, CalendarDto>(
                sql,
                map: (calendar, singleDate, recurringDate) =>
                {
                    if (!calendarDict.TryGetValue(calendar.CalendarId, out var calendarEntry))
                    {
                        calendarEntry = calendar;
                        calendarDict.Add(calendar.CalendarId, calendarEntry);
                    }

                    if (singleDate is not null && calendarEntry.SingleDates.All(x => x.SingleDateId != singleDate.SingleDateId))
                    {
                        calendarEntry.SingleDates.Add(singleDate);
                    }

                    if (recurringDate is not null && calendarEntry.RecurringDates.All(x => x.RecurringDateId != recurringDate.RecurringDateId))
                    {
                        calendarEntry.RecurringDates.Add(recurringDate);
                    }

                    return calendarEntry;
                },
                param: new { CalendarIds = calendarIds },
                splitOn: "SingleDateId,RecurringDateId");

            return calendarDict.Values.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting selected calendars from database");
            return [];
        }
    }
}
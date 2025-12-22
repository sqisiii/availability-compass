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
            const string sql = @"
                        SELECT c.CalendarId, c.Name, c.IsOnly, c.ChangeDate,
                            sd.Id as SingleDateId, sd.CalendarId, sd.Description as SingleDateDescription, sd.Date, sd.ChangeDate,
                            rd.Id as RecurringDateId, rd.CalendarId, rd.Description as RecurringDateDescription, rd.StartDate, rd.Duration, rd.Frequency, rd.NumberOfRepetitions, rd.ChangeDate
                        FROM Calendar c
                        LEFT JOIN SingleDate sd ON c.CalendarId = sd.CalendarId
                        LEFT JOIN RecurringDate rd ON c.CalendarId = rd.CalendarId";

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
                    splitOn: "SingleDateId,RecurringDateId")
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
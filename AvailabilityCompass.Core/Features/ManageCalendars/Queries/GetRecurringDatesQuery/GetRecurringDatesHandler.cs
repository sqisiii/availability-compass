using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetRecurringDatesQuery;

public class GetRecurringDatesHandler : IRequestHandler<GetRecurringDatesQuery, GetRecurringDatesResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetRecurringDatesHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<GetRecurringDatesResponse> Handle(GetRecurringDatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            const string sql = @"
                        SELECT rd.CalendarId, rd.Id as RecurringDateId, rd.Description as RecurringDateDescription, rd.StartDate, rd.Duration, rd.RepetitionPeriod, rd.NumberOfRepetitions, rd.ChangeDate
                        FROM RecurringDate rd
                        WHERE rd.CalendarId = @Id";

            var recurringDates = await connection.QueryAsync<RecurringDateDto>(sql, new { Id = request.CalendarId }).ConfigureAwait(false);

            return new GetRecurringDatesResponse(recurringDates.ToList(), true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while getting recurring dates");
            return new GetRecurringDatesResponse([], false);
        }
    }
}
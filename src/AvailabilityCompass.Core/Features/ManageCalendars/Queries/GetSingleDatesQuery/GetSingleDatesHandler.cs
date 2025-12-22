using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetSingleDatesQuery;

public class GetSingleDatesHandler : IRequestHandler<GetSingleDatesQuery, GetSingleDatesResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetSingleDatesHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<GetSingleDatesResponse> Handle(GetSingleDatesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            // language=SQLite
            const string sql = @"
            SELECT sd.CalendarId, sd.Id as SingleDateId, sd.Description as SingleDateDescription, sd.Date, sd.ChangeDate
            FROM SingleDate sd
            WHERE sd.CalendarId = @Id";

            var singleDates = await connection.QueryAsync<SingleDateDto>(sql, new { Id = request.CalendarId }).ConfigureAwait(false);

            return new GetSingleDatesResponse(singleDates.ToList(), true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while getting single dates");
            return new GetSingleDatesResponse([], false);
        }
    }
}
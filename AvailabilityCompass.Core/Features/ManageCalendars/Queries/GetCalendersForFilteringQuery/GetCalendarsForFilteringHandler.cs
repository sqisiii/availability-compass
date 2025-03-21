using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetCalendars;
using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendersForFilteringQuery;

public class GetCalendarsForFilteringHandler : IRequestHandler<GetCalendarsForFilteringQuery, GetCalendarsForFilteringResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetCalendarsForFilteringHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<GetCalendarsForFilteringResponse> Handle(GetCalendarsForFilteringQuery request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            const string sql = @"
                        SELECT c.CalendarId as Id , c.Name, c.IsOnly, c.ChangeDate
                        FROM Calendar c";

            var calendars = await connection.QueryAsync<GetCalendarsForFilteringResponse.CalendarDto>(sql);
            return new GetCalendarsForFilteringResponse(calendars.ToList(), true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while getting calendars for filtering");
            throw;
        }
    }
}
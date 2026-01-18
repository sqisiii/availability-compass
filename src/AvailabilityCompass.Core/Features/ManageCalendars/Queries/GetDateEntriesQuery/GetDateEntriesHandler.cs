using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetDateEntriesQuery;

/// <summary>
/// Handles the get date entries query by retrieving all date entries for a calendar from the database.
/// </summary>
public class GetDateEntriesHandler : IRequestHandler<GetDateEntriesQuery, GetDateEntriesResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetDateEntriesHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<GetDateEntriesResponse> Handle(GetDateEntriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            // language=SQLite
            const string sql = """
                               SELECT
                                de.CalendarId,
                                de.Id AS DateEntryId,
                                de.StartDate,
                                de.Description,
                                de.IsRecurring,
                                de.Duration,
                                de.Frequency,
                                de.NumberOfRepetitions,
                                de.ChangeDate
                               FROM DateEntry de
                               WHERE de.CalendarId = @CalendarId
                               ORDER BY de.StartDate
                               """;

            var dateEntries = await connection.QueryAsync<DateEntryDto>(sql, new { request.CalendarId }).ConfigureAwait(false);

            return new GetDateEntriesResponse(dateEntries.ToList(), true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while getting date entries");
            return new GetDateEntriesResponse([], false);
        }
    }
}
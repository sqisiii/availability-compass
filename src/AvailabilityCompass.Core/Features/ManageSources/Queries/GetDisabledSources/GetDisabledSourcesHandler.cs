using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetDisabledSources;

/// <summary>
/// Handles the get disabled sources query by retrieving all disabled source IDs from the database.
/// </summary>
public class GetDisabledSourcesHandler : IRequestHandler<GetDisabledSourcesQuery, GetDisabledSourcesResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetDisabledSourcesHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<GetDisabledSourcesResponse> Handle(GetDisabledSourcesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            // language=SQLite
            const string query = "SELECT SourceId FROM DisabledSources;";

            var sourceIds = await connection.QueryAsync<string>(query).ConfigureAwait(false);

            return new GetDisabledSourcesResponse(sourceIds.ToHashSet());
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to get disabled sources from the database");
        }

        return new GetDisabledSourcesResponse([]);
    }
}

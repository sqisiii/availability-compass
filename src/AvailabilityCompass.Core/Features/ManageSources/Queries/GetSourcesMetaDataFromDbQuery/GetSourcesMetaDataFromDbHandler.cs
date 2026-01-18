using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;

/// <summary>
/// Handles the get sources metadata query by retrieving summary information from the database.
/// </summary>
public class GetSourcesMetaDataFromDbHandler : IRequestHandler<GetSourcesMetaDataFromDbQuery, IEnumerable<GetSourcesMetaDataFromDbDto>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetSourcesMetaDataFromDbHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<GetSourcesMetaDataFromDbDto>> Handle(
        GetSourcesMetaDataFromDbQuery request,
        CancellationToken cancellationToken)
    {
        return await GetSourcesSummariesAsync();
    }

    private async Task<IEnumerable<GetSourcesMetaDataFromDbDto>> GetSourcesSummariesAsync()
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            // language=SQLite
            const string query = """
                                 SELECT SourceId, 
                                 MAX(ChangeDate) AS ChangedAt, 
                                 COUNT(SeqNo) AS TripsCount
                                 FROM Source
                                 GROUP BY SourceId;
                                 """;

            return await connection.QueryAsync<GetSourcesMetaDataFromDbDto>(query).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to get sources metadata from the database");
        }

        return [];
    }
}
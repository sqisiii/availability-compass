using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;

public class GetSourcesMetaDataFromDbHandler : IRequestHandler<GetSourcesMetaDataFromDbQuery, IEnumerable<GetSourcesMetaDataFromDbDto>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetSourcesMetaDataFromDbHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<GetSourcesMetaDataFromDbDto>> Handle(GetSourcesMetaDataFromDbQuery request, CancellationToken cancellationToken)
    {
        return await GetSourcesSummariesAsync();
    }

    private async Task<IEnumerable<GetSourcesMetaDataFromDbDto>> GetSourcesSummariesAsync()
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            var query = @"
                        SELECT 
                            SourceId, 
                            MAX(ChangeDate) AS ChangedAt, 
                            COUNT(SeqNo) AS TripsCount
                        FROM Source
                        GROUP BY SourceId;";

            return await connection.QueryAsync<GetSourcesMetaDataFromDbDto>(query);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to get sources metadata from the database");
        }

        return [];
    }
}
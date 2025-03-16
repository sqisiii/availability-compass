using AvailabilityCompass.Core.Features.ManageSources.Sources;
using AvailabilityCompass.Core.Features.Search.Queries.GetSources;
using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesForFilteringQuery;

public class GetSourcesForFilteringHandler : IRequestHandler<Search.Queries.GetSources.GetSourcesForFilteringQuery, GetSourcesForFilteringResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ISourceStore _sourceStore;

    public GetSourcesForFilteringHandler(IDbConnectionFactory dbConnectionFactory, ISourceStore sourceStore)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _sourceStore = sourceStore;
    }

    public async Task<GetSourcesForFilteringResponse> Handle(Search.Queries.GetSources.GetSourcesForFilteringQuery request, CancellationToken cancellationToken)
    {
        var response = new GetSourcesForFilteringResponse();
        var sourcesData = _sourceStore.GetSourceData();
        var sourceChangeAtDates = (await GetSourceChangedAtDatesAsync()).ToList();

        foreach (var sourceData in sourcesData.OrderBy(i => i.Name))
        {
            var sourceChangeAtDate = sourceChangeAtDates.FirstOrDefault(x => x.SourceId == sourceData.Id).ChangedAt;
            response.Sources.Add(new GetSourcesForFilteringResponse.Source
            {
                Name = sourceData.Name,
                ChangedAt = sourceChangeAtDate,
                IsEnabled = sourceData.IsEnabled
            });
        }

        return response;
    }

    private async Task<IEnumerable<(string SourceId, DateTime ChangedAt)>> GetSourceChangedAtDatesAsync()
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            var query = @"
                        SELECT 
                            SourceId, 
                            MAX(ChangeDate) AS ChangedAt 
                        FROM Source
                        GROUP BY SourceId;";

            return await connection.QueryAsync<(string SourceId, DateTime ChangedAt)>(query);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to get changed at dates from the database");
        }

        return [];
    }
}
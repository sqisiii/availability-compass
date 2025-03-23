using AvailabilityCompass.Core.Features.ManageSources.Sources;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetSources;
using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesForFilteringQuery;

public class GetSourcesForFilteringHandler : IRequestHandler<
    SearchRecords.Queries.GetSources.GetSourcesForFilteringQuery, GetSourcesForFilteringResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISourceStore _sourceStore;

    public GetSourcesForFilteringHandler(IDbConnectionFactory dbConnectionFactory, ISourceStore sourceStore, IServiceProvider serviceProvider)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _sourceStore = sourceStore;
        _serviceProvider = serviceProvider;
    }

    public async Task<GetSourcesForFilteringResponse> Handle(SearchRecords.Queries.GetSources.GetSourcesForFilteringQuery request, CancellationToken cancellationToken)
    {
        var response = new GetSourcesForFilteringResponse();
        var sourcesData = _sourceStore.GetSourceMetaData();
        var sourceChangeAtDates = (await GetSourceChangedAtDatesAsync()).ToList();

        foreach (var sourceData in sourcesData.OrderBy(i => i.Name))
        {
            var sourceChangeAtDate = sourceChangeAtDates.FirstOrDefault(x => x.SourceId == sourceData.Id).ChangedAt;

            var sourceService = _serviceProvider.GetKeyedService<ISourceService>(sourceData.Id);
            if (sourceService is null)
            {
                Log.Debug("Failed to get source service for source {SourceId}", sourceData.Id);
                continue;
            }

            var filterOptions = new List<SourceFilter>();
            try
            {
                filterOptions.AddRange(await sourceService.GetFilters(cancellationToken));
            }
            catch (Exception e)
            {
                Log.Debug(e, "Failed to get filter options for source {SourceId}", sourceData.Id);
            }

            var source = new GetSourcesForFilteringResponse.Source
            {
                SourceId = sourceData.Id,
                Name = sourceData.Name,
                ChangedAt = sourceChangeAtDate,
                IsEnabled = sourceData.IsEnabled,
                Filters = filterOptions.Select(f => new GetSourcesForFilteringResponse.SourceFilter
                    {
                        Label = f.Label,
                        Type = f.Type switch
                        {
                            SourceFilterType.Boolean => GetSourcesForFilteringResponse.SourceFilterType.Boolean,
                            SourceFilterType.Text => GetSourcesForFilteringResponse.SourceFilterType.Text,
                            SourceFilterType.MultiSelect => GetSourcesForFilteringResponse.SourceFilterType.MultiSelect,
                            _ => GetSourcesForFilteringResponse.SourceFilterType.Text
                        },
                        Options = f.Options
                    })
                    .ToList()
            };
            response.Sources.Add(source);
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
using AvailabilityCompass.Core.Features.ManageSources.Integrations;
using AvailabilityCompass.Core.Features.Search.Queries.GetSources;
using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesForFilteringQuery;

public class GetSourcesForFilteringHandler : IRequestHandler<Search.Queries.GetSources.GetSourcesForFilteringQuery, GetSourcesForFilteringResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IIntegrationStore _integrationStore;

    public GetSourcesForFilteringHandler(IDbConnectionFactory dbConnectionFactory, IIntegrationStore integrationStore)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _integrationStore = integrationStore;
    }

    public async Task<GetSourcesForFilteringResponse> Handle(Search.Queries.GetSources.GetSourcesForFilteringQuery request, CancellationToken cancellationToken)
    {
        var response = new GetSourcesForFilteringResponse();
        var integrationsData = _integrationStore.GetIntegrationsIdAndNames();
        var integrationChangedAtDates = (await GetIntegrationChangedAtDatesAsync()).ToList();

        foreach (var integrationData in integrationsData.OrderBy(i => i.IntegrationName))
        {
            var integrationChangedAt = integrationChangedAtDates.FirstOrDefault(x => x.IntegrationId == integrationData.IntegrationId).ChangedAt;
            response.Sources.Add(new GetSourcesForFilteringResponse.Source
            {
                Name = integrationData.IntegrationName,
                ChangedAt = integrationChangedAt,
                IsEnabled = integrationData.IntegrationEnabled
            });
        }

        return response;
    }

    private async Task<IEnumerable<(string IntegrationId, DateTime ChangedAt)>> GetIntegrationChangedAtDatesAsync()
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            var query = @"
                        SELECT 
                            IntegrationId, 
                            MAX(ChangeDate) AS ChangedAt 
                        FROM Source
                        GROUP BY IntegrationId;";

            return await connection.QueryAsync<(string IntegrationId, DateTime ChangedAt)>(query);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to get changed at dates from the database");
        }

        return [];
    }
}
using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;

public class ReplaceSourceDataInDbHandler : IRequestHandler<ReplaceSourceDataInDbRequest, ReplaceSourceDataInDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ReplaceSourceDataInDbHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<ReplaceSourceDataInDbResponse> Handle(ReplaceSourceDataInDbRequest request, CancellationToken cancellationToken)
    {
        var result = await ReplaceSourcesAsync(request.Sources);

        return new ReplaceSourceDataInDbResponse(result);
    }

    private async Task<bool> ReplaceSourcesAsync(IReadOnlyCollection<SourceDataItem> sources)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var integrationIds = sources.Select(s => s.IntegrationId).Distinct().ToList();

            if (integrationIds.Count == 0)
            {
                Log.Warning("There are no sources to insert");
                return false; // No sources to insert
            }

            const string deleteSourceSql = @"DELETE FROM Source WHERE IntegrationId IN @IntegrationIds;";
            await connection.ExecuteAsync(
                    deleteSourceSql,
                    new { IntegrationIds = integrationIds }, transaction)
                .ConfigureAwait(false);

            const string deleteAdditionalDataSql = @"DELETE FROM SourceAdditionalData WHERE IntegrationId IN @IntegrationIds;";
            await connection.ExecuteAsync(
                    deleteAdditionalDataSql,
                    new { IntegrationIds = integrationIds }, transaction)
                .ConfigureAwait(false);
            var sourceInserts = new List<object>();
            var additionalDataInserts = new List<object>();

            foreach (var source in integrationIds.SelectMany(integrationId => sources.Where(s => s.IntegrationId == integrationId)))
            {
                sourceInserts.Add(new
                {
                    source.SeqNo,
                    source.IntegrationId,
                    source.Title,
                    source.Type,
                    source.Country,
                    source.StartDate,
                    source.EndDate,
                    source.Price,
                    source.ChangeDate
                });

                foreach (var (key, value) in source.AdditionalData)
                {
                    additionalDataInserts.Add(new
                    {
                        SourceSeqNo = source.SeqNo,
                        source.IntegrationId,
                        Key = key,
                        Value = value
                    });
                }
            }

            if (sourceInserts.Any())
            {
                var sourceUpdateSql = @"INSERT INTO Source (SeqNo, IntegrationId, Title, Type, Country, StartDate, EndDate, Price, ChangeDate) 
                    VALUES (@SeqNo, @IntegrationId, @Title, @Type, @Country, @StartDate, @EndDate, @Price, @ChangeDate);";
                await connection.ExecuteAsync(sourceUpdateSql, sourceInserts, transaction).ConfigureAwait(false);
            }

            if (additionalDataInserts.Any())
            {
                var additionalDataUpdateSql = @"INSERT INTO SourceAdditionalData (SourceSeqNo, IntegrationId, Key, Value) 
                    VALUES (@SourceSeqNo, @IntegrationId, @Key, @Value);";
                await connection.ExecuteAsync(additionalDataUpdateSql, additionalDataInserts, transaction).ConfigureAwait(false);
            }

            transaction.Commit();
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to add sources");
            return false;
        }

        return true;
    }
}
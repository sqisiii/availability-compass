using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;

public class ReplaceSourceDataInDbHandler : IRequestHandler<ReplaceSourceDataInDbRequest, ReplaceSourceDataInDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public ReplaceSourceDataInDbHandler(IDbConnectionFactory dbConnectionFactory, IEventBus eventBus)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _eventBus = eventBus;
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

            var sourceIds = sources.Select(s => s.SourceId).Distinct().ToList();

            if (sourceIds.Count == 0)
            {
                Log.Warning("There are no sources to insert");
                return false; // No sources to insert
            }

            const string deleteSourceSql = @"DELETE FROM Source WHERE SourceId IN @SourceIds;";
            await connection.ExecuteAsync(
                    deleteSourceSql,
                    new { SourceIds = sourceIds }, transaction)
                .ConfigureAwait(false);

            const string deleteAdditionalDataSql = @"DELETE FROM SourceAdditionalData WHERE SourceId IN @SourcesIds;";
            await connection.ExecuteAsync(
                    deleteAdditionalDataSql,
                    new { SourcesIds = sourceIds }, transaction)
                .ConfigureAwait(false);
            var sourceInserts = new List<object>();
            var additionalDataInserts = new List<object>();

            foreach (var source in sourceIds.SelectMany(sourceId => sources.Where(s => s.SourceId == sourceId)))
            {
                sourceInserts.Add(new
                {
                    source.SeqNo,
                    source.SourceId,
                    source.Title,
                    source.Url,
                    source.StartDate,
                    source.EndDate,
                    source.ChangeDate
                });

                foreach (var (key, value) in source.AdditionalData)
                {
                    additionalDataInserts.Add(new
                    {
                        SourceSeqNo = source.SeqNo,
                        source.SourceId,
                        Key = key,
                        Value = value
                    });
                }
            }

            if (sourceInserts.Any())
            {
                var sourceUpdateSql = @"INSERT INTO Source (SeqNo, SourceId, Title, Url, StartDate, EndDate, ChangeDate) 
                    VALUES (@SeqNo, @SourceId, @Title, @Url, @StartDate, @EndDate, @ChangeDate);";
                await connection.ExecuteAsync(sourceUpdateSql, sourceInserts, transaction).ConfigureAwait(false);
            }

            if (additionalDataInserts.Any())
            {
                var additionalDataUpdateSql = @"INSERT INTO SourceAdditionalData (SourceSeqNo, SourceId, Key, Value) 
                    VALUES (@SourceSeqNo, @SourceId, @Key, @Value);";
                await connection.ExecuteAsync(additionalDataUpdateSql, additionalDataInserts, transaction).ConfigureAwait(false);
            }

            transaction.Commit();
            _eventBus.Publish(new SourcesDataChangedEvent());
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to add sources");
            return false;
        }

        return true;
    }
}
using AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;
using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Commands.SetSourceDisabled;

/// <summary>
/// Handles the set source disabled command by inserting or deleting from the DisabledSources table.
/// </summary>
public class SetSourceDisabledHandler : IRequestHandler<SetSourceDisabledRequest, SetSourceDisabledResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public SetSourceDisabledHandler(IDbConnectionFactory dbConnectionFactory, IEventBus eventBus)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _eventBus = eventBus;
    }

    public async Task<SetSourceDisabledResponse> Handle(SetSourceDisabledRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            if (request.IsDisabled)
            {
                // language=SQLite
                const string insertSql = """
                                         INSERT OR REPLACE INTO DisabledSources (SourceId, ChangeDate)
                                         VALUES (@SourceId, @ChangeDate);
                                         """;

                var changeDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

                await connection.ExecuteAsync(insertSql, new
                    {
                        request.SourceId,
                        ChangeDate = changeDate
                    })
                    .ConfigureAwait(false);
            }
            else
            {
                // language=SQLite
                const string deleteSql = "DELETE FROM DisabledSources WHERE SourceId = @SourceId;";

                await connection.ExecuteAsync(deleteSql, new { request.SourceId })
                    .ConfigureAwait(false);
            }

            _eventBus.Publish(new SourcesDataChangedEvent());
            return new SetSourceDisabledResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error setting source {SourceId} disabled state to {IsDisabled}", request.SourceId, request.IsDisabled);
            return new SetSourceDisabledResponse(false);
        }
    }
}

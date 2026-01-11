using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteDateEntryRequest;

public class DeleteDateEntryFromDbHandler : IRequestHandler<DeleteDateEntryFromDbRequest, DeleteDateEntryFromDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public DeleteDateEntryFromDbHandler(IDbConnectionFactory dbConnectionFactory, IEventBus eventBus)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _eventBus = eventBus;
    }

    public async Task<DeleteDateEntryFromDbResponse> Handle(DeleteDateEntryFromDbRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            // language=SQLite
            const string sql = "DELETE FROM DateEntry WHERE CalendarId = @CalendarId AND Id = @DateEntryId;";
            await connection.ExecuteAsync(sql, new { request.CalendarId, request.DateEntryId }).ConfigureAwait(false);

            _eventBus.Publish(new DateEntryDeletedEvent(request.CalendarId));
            return new DeleteDateEntryFromDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting date entry from database");
            return new DeleteDateEntryFromDbResponse(false);
        }
    }
}
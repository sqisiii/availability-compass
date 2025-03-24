using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteSingleDateRequest;

public class DeleteSingleDateFromDbHandler : IRequestHandler<DeleteSingleDateFromDbRequest, DeleteSingleDateFromDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public DeleteSingleDateFromDbHandler(IEventBus eventBus, IDbConnectionFactory dbConnectionFactory)
    {
        _eventBus = eventBus;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<DeleteSingleDateFromDbResponse> Handle(DeleteSingleDateFromDbRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            //enable automatic foreign key enforcement with cascade delete
            await connection.ExecuteAsync("PRAGMA foreign_keys = ON;").ConfigureAwait(false);

            const string sql = @"DELETE FROM SingleDate WHERE CalendarId = @CalendarId and Id = @SingleDateId;";
            await connection.ExecuteAsync(sql, new { request.CalendarId, request.SingleDateId }).ConfigureAwait(false);

            _eventBus.Publish(new SingleDateDeletedEvent(request.CalendarId));
            return new DeleteSingleDateFromDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting single date from database");
            return new DeleteSingleDateFromDbResponse(false);
        }
    }
}
using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteRecurringDateRequest;

public class DeleteRecurringDateFromDbHandler : IRequestHandler<DeleteRecurringDateFromDbRequest, DeleteRecurringDateFromDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public DeleteRecurringDateFromDbHandler(IEventBus eventBus, IDbConnectionFactory dbConnectionFactory)
    {
        _eventBus = eventBus;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<DeleteRecurringDateFromDbResponse> Handle(DeleteRecurringDateFromDbRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            //enable automatic foreign key enforcement with cascade delete
            await connection.ExecuteAsync("PRAGMA foreign_keys = ON;").ConfigureAwait(false);

            const string sql = @"DELETE FROM RecurringDate WHERE CalendarId = @CalendarId and Id = @RecurringDateId;";
            await connection.ExecuteAsync(sql, new { request.CalendarId, request.RecurringDateId }).ConfigureAwait(false);

            _eventBus.Publish(new RecurringDateDeletedEvent(request.CalendarId));
            return new DeleteRecurringDateFromDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting recurring date from database");
            return new DeleteRecurringDateFromDbResponse(false);
        }
    }
}
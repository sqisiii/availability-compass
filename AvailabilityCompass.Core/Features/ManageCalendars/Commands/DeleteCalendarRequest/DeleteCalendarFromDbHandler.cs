using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteCalendarRequest;

public class DeleteCalendarFromDbHandler : IRequestHandler<DeleteCalendarFromDbRequest, DeleteCalendarFromDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public DeleteCalendarFromDbHandler(IEventBus eventBus, IDbConnectionFactory dbConnectionFactory)
    {
        _eventBus = eventBus;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<DeleteCalendarFromDbResponse> Handle(DeleteCalendarFromDbRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            //enable automatic foreign key enforcement = deletion
            await connection.ExecuteAsync("PRAGMA foreign_keys = ON;").ConfigureAwait(false);

            const string deleteCalendarSql = @"DELETE FROM Calendar WHERE CalendarId = @CalendarId;";
            await connection.ExecuteAsync(deleteCalendarSql, new { request.CalendarId }).ConfigureAwait(false);

            _eventBus.Publish(new CalendarDeletedEvent());
            return new DeleteCalendarFromDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting calendar from database");
            return new DeleteCalendarFromDbResponse(false);
        }
    }
}
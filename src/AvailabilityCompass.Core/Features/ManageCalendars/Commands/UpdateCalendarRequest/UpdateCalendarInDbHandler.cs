using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;

public class UpdateCalendarInDbHandler : IRequestHandler<UpdateCalendarInDbRequest, UpdateCalendarInDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public UpdateCalendarInDbHandler(IDbConnectionFactory dbConnectionFactory, IEventBus eventBus)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _eventBus = eventBus;
    }

    public async Task<UpdateCalendarInDbResponse> Handle(UpdateCalendarInDbRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            var changeDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            // language=SQLite
            await connection.ExecuteAsync(
                "UPDATE Calendar SET Name = @Name, IsOnly = @IsOnly, ChangeDate = @ChangeDate WHERE CalendarId = @CalendarId",
                new { request.CalendarId, request.Name, request.IsOnly, ChangeDate = changeDate }
            ).ConfigureAwait(false);

            _eventBus.Publish(new CalendarUpdatedEvent());
            return new UpdateCalendarInDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating calendar in database");
            return new UpdateCalendarInDbResponse(false);
        }
    }
}
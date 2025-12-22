using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateSingleDateRequest;

public class UpdateSingleDateInDbHandler : IRequestHandler<UpdateSingleDateInDbRequest, UpdateSingleDateInDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public UpdateSingleDateInDbHandler(IDbConnectionFactory dbConnectionFactory, IEventBus eventBus)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _eventBus = eventBus;
    }


    public async Task<UpdateSingleDateInDbResponse> Handle(UpdateSingleDateInDbRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            var changeDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            // language=SQLite
            await connection.ExecuteAsync(
                "UPDATE SingleDate SET Description = @Description, Date = @Date, ChangeDate = @ChangeDate WHERE Id = @SingleDateId AND CalendarId = @CalendarId",
                new { request.SingleDateId, request.CalendarId, request.Date, request.Description, ChangeDate = changeDate }
            ).ConfigureAwait(false);

            _eventBus.Publish(new SingleDateUpdatedEvent(request.CalendarId));
            return new UpdateSingleDateInDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating single date in database");
            return new UpdateSingleDateInDbResponse(false);
        }
    }
}
using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateRecurringDateRequest;

public class UpdateRecurringDateInDbHandler : IRequestHandler<UpdateRecurringDateInDbRequest, UpdateRecurringDateInDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public UpdateRecurringDateInDbHandler(IDbConnectionFactory dbConnectionFactory, IEventBus eventBus)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _eventBus = eventBus;
    }


    public async Task<UpdateRecurringDateInDbResponse> Handle(UpdateRecurringDateInDbRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            var changeDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            await connection.ExecuteAsync(
                "UPDATE RecurringDate SET StartDate = @StartDate, Duration = @Duration, RepetitionPeriod = @RepetitionPeriod, " +
                "NumberOfRepetitions = @NumberOfRepetitions, Description = @Description, ChangeDate = @ChangeDate " +
                "WHERE Id = @RecurringDateId AND CalendarId = @CalendarId",
                new
                {
                    request.RecurringDateId,
                    request.CalendarId,
                    request.StartDate,
                    request.Duration,
                    request.RepetitionPeriod,
                    request.NumberOfRepetitions,
                    request.Description,
                    ChangeDate = changeDate
                }
            );

            _eventBus.Publish(new RecurringDateUpdatedEvent(request.CalendarId));
            return new UpdateRecurringDateInDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating recurring date in database");
            return new UpdateRecurringDateInDbResponse(false);
        }
    }
}
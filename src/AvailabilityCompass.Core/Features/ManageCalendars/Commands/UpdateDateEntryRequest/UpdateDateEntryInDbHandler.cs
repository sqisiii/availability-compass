using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateDateEntryRequest;

public class UpdateDateEntryInDbHandler : IRequestHandler<UpdateDateEntryInDbRequest, UpdateDateEntryInDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public UpdateDateEntryInDbHandler(IDbConnectionFactory dbConnectionFactory, IEventBus eventBus)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _eventBus = eventBus;
    }

    public async Task<UpdateDateEntryInDbResponse> Handle(UpdateDateEntryInDbRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            var changeDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            // language=SQLite
            const string updateSql = """
                                     UPDATE DateEntry
                                     SET StartDate = @StartDate,
                                     Description = @Description,
                                     IsRecurring = @IsRecurring,
                                     Duration = @Duration,
                                     Frequency = @Frequency,
                                     NumberOfRepetitions = @NumberOfRepetitions,
                                     ChangeDate = @ChangeDate
                                     WHERE Id = @DateEntryId AND CalendarId = @CalendarId
                                     """;

            await connection.ExecuteAsync(updateSql, new
                {
                    request.DateEntryId,
                    request.CalendarId,
                    request.StartDate,
                    request.Description,
                    request.IsRecurring,
                    request.Duration,
                    request.Frequency,
                    request.NumberOfRepetitions,
                    ChangeDate = changeDate
                })
                .ConfigureAwait(false);

            _eventBus.Publish(new DateEntryUpdatedEvent(request.CalendarId));
            return new UpdateDateEntryInDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating date entry in database");
            return new UpdateDateEntryInDbResponse(false);
        }
    }
}
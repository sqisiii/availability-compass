using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddDateEntryRequest;

/// <summary>
/// Handles the add date entry command by inserting a new date entry into the database.
/// </summary>
public class AddDateEntryToDbHandler : IRequestHandler<AddDateEntryToDbRequest, AddDateEntryToDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public AddDateEntryToDbHandler(IDbConnectionFactory dbConnectionFactory, IEventBus eventBus)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _eventBus = eventBus;
    }

    public async Task<AddDateEntryToDbResponse> Handle(AddDateEntryToDbRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            // language=SQLite
            const string insertDateEntrySql = """
                                              INSERT INTO DateEntry (CalendarId, Id, StartDate, Description, IsRecurring, Duration, Frequency, NumberOfRepetitions, ChangeDate)
                                              VALUES (@CalendarId, @Id, @StartDate, @Description, @IsRecurring, @Duration, @Frequency, @NumberOfRepetitions, @ChangeDate);
                                              """;

            var id = Guid.NewGuid();
            var changeDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            await connection.ExecuteAsync(insertDateEntrySql, new
                {
                    request.CalendarId,
                    Id = id,
                    request.StartDate,
                    request.Description,
                    request.IsRecurring,
                    request.Duration,
                    request.Frequency,
                    request.NumberOfRepetitions,
                    ChangeDate = changeDate
                })
                .ConfigureAwait(false);

            _eventBus.Publish(new DateEntryAddedEvent(request.CalendarId));
            return new AddDateEntryToDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error adding date entry to database");
            return new AddDateEntryToDbResponse(false);
        }
    }
}
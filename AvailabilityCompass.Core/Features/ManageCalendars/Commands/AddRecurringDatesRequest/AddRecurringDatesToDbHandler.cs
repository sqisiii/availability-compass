using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddRecurringDatesRequest;

public class AddRecurringDatesToDbHandler : IRequestHandler<AddRecurringDatesToDbRequest, AddRecurringDatesToDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public AddRecurringDatesToDbHandler(IDbConnectionFactory dbConnectionFactory, IEventBus eventBus)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _eventBus = eventBus;
    }

    public async Task<AddRecurringDatesToDbResponse> Handle(AddRecurringDatesToDbRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            const string insertSingleDateSql = @"INSERT INTO RecurringDate (CalendarId, Id, StartDate, Description, Duration, RepetitionPeriod, NumberOfRepetitions, ChangeDate) 
                                                VALUES (@CalendarId, @Id, @StartDate, @Description, @Duration, @RepetitionPeriod, @NumberOfRepetitions, @ChangeDate);";

            var id = Guid.NewGuid();
            var changeDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            await connection.ExecuteAsync(insertSingleDateSql, new
                {
                    request.CalendarId,
                    Id = id,
                    request.StartDate,
                    request.Description,
                    request.Duration,
                    request.RepetitionPeriod,
                    request.NumberOfRepetitions,
                    ChangeDate = changeDate
                })
                .ConfigureAwait(false);

            _eventBus.Publish(new RecurringDateAddedEvent(request.CalendarId));
            return new AddRecurringDatesToDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error adding calendar recurring date to database");
            return new AddRecurringDatesToDbResponse(false);
        }
    }
}
using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;

public class AddCalendarToDbHandler : IRequestHandler<AddCalendarToDbRequest, AddCalendarToDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public AddCalendarToDbHandler(IDbConnectionFactory dbConnectionFactory, IEventBus eventBus)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _eventBus = eventBus;
    }

    public async Task<AddCalendarToDbResponse> Handle(AddCalendarToDbRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            // language=SQLite
            const string insertCalendarSql = @"INSERT INTO Calendar (CalendarId, Name, IsOnly, ChangeDate) 
                                                VALUES (@CalendarId, @Name, @IsOnly, @ChangeDate);";

            var calendarId = Guid.NewGuid();
            var changeDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            await connection.ExecuteAsync(insertCalendarSql, new
                {
                    CalendarId = calendarId,
                    request.Name,
                    request.IsOnly,
                    ChangeDate = changeDate
                })
                .ConfigureAwait(false);

            _eventBus.Publish(new CalendarAddedEvent(calendarId));
            return new AddCalendarToDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error adding calendar to database");
            return new AddCalendarToDbResponse(false);
        }
    }
}
using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddSingleDateRequest;

public class AddSingleDateToDbHandler : IRequestHandler<AddSingleDateToDbRequest, AddSingleDateToDbResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IEventBus _eventBus;

    public AddSingleDateToDbHandler(IDbConnectionFactory dbConnectionFactory, IEventBus eventBus)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _eventBus = eventBus;
    }

    public async Task<AddSingleDateToDbResponse> Handle(AddSingleDateToDbRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            const string insertSingleDateSql = @"INSERT INTO SingleDate (CalendarId, Id, Date, Description, ChangeDate) 
                                                VALUES (@CalendarId, @Id, @Date, @Description, @ChangeDate);";

            var id = Guid.NewGuid();
            var changeDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            await connection.ExecuteAsync(insertSingleDateSql, new
                {
                    request.CalendarId,
                    Id = id,
                    request.Date,
                    request.Description,
                    ChangeDate = changeDate
                })
                .ConfigureAwait(false);

            _eventBus.Publish(new SingleDateAddedEvent(request.CalendarId));
            return new AddSingleDateToDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error adding calendar single date to database");
            return new AddSingleDateToDbResponse(false);
        }
    }
}
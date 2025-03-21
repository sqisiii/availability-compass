using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.Core.Shared.EventBus;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarSingleDateRequest;

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
            using var transaction = connection.BeginTransaction();

            const string insertSingleDateSql = @"INSERT INTO SingleDate (CalendarId, Id, Date, Description) 
                                                VALUES (@CalendarId, @Id, @Date, @Description);";

            var id = Guid.NewGuid();
            var changeDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            await connection.ExecuteAsync(insertSingleDateSql, new
                {
                    request.CalendarId,
                    Id = id,
                    request.Date,
                    request.Description
                }, transaction)
                .ConfigureAwait(false);

            transaction.Commit();

            _eventBus.Publish(new SingleDateAddedEvent());
            return new AddSingleDateToDbResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error adding calendar single date to database");
            return new AddSingleDateToDbResponse(false);
        }
    }
}
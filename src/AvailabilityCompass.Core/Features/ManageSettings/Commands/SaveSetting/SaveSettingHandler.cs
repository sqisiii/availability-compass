using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSettings.Commands.SaveSetting;

public class SaveSettingHandler : IRequestHandler<SaveSettingRequest, SaveSettingResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SaveSettingHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<SaveSettingResponse> Handle(SaveSettingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();
            // language=SQLite
            const string upsertSql = @"INSERT OR REPLACE INTO Setting (Key, Value, ChangeDate)
                                       VALUES (@Key, @Value, @ChangeDate);";

            var changeDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            await connection.ExecuteAsync(upsertSql, new
                {
                    request.Key,
                    request.Value,
                    ChangeDate = changeDate
                })
                .ConfigureAwait(false);

            return new SaveSettingResponse(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving setting {Key} to database", request.Key);
            return new SaveSettingResponse(false);
        }
    }
}
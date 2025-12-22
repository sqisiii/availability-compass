using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSettings.Queries.GetSetting;

public class GetSettingHandler : IRequestHandler<GetSettingQuery, GetSettingResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetSettingHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<GetSettingResponse> Handle(GetSettingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();
            // language=SQLite
            const string sql = @"SELECT Value FROM Setting WHERE Key = @Key";

            var value = await connection.QueryFirstOrDefaultAsync<string>(sql, new { request.Key })
                .ConfigureAwait(false);

            if (value is not null)
            {
                return new GetSettingResponse(value, true);
            }

            return new GetSettingResponse(request.DefaultValue, false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting setting {Key} from database", request.Key);
            return new GetSettingResponse(request.DefaultValue, false);
        }
    }
}
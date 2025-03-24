using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetFilterOptionsQuery;

public class GetFilterOptionsHandler : IRequestHandler<GetFilterOptionsQuery, GetFilterOptionsResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetFilterOptionsHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<GetFilterOptionsResponse> Handle(GetFilterOptionsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            var filterOptions = new Dictionary<string, List<string>>();

            foreach (var fieldName in query.FieldNames)
            {
                const string sql = $@"
                        SELECT DISTINCT Value 
                        FROM SourceAdditionalData
                    WHERE SourceId = @SourceId and Key = @Key";
                var parameters = new { SourceId = query.SourceId, Key = fieldName };
                var values = await connection.QueryAsync<string>(sql, parameters).ConfigureAwait(false);
                filterOptions[fieldName] = values.OrderBy(x => x).ToList();
            }

            var response = new GetFilterOptionsResponse
            {
                FilterOptions = filterOptions
            };
            return response;
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to get Horyzonty options from the database");
        }

        return new GetFilterOptionsResponse();
    }
}
using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources.Horyzonty.Queries.GetHoryzontyFilterOptionsQuery;

public class GetHoryzontyFilterOptionsHandler : IRequestHandler<GetHoryzontyFilterOptionsQuery, GetHoryzontyFilterOptionsResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetHoryzontyFilterOptionsHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<GetHoryzontyFilterOptionsResponse> Handle(GetHoryzontyFilterOptionsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            connection.Open();

            var getCountriesSql = @"
                        SELECT DISTINCT Value 
                        FROM SourceAdditionalData
                        WHERE SourceId = @SourceId and Key = 'Country'";
            var getCountriesParameters = new { SourceId = query.SourceId };
            var countries = await connection.QueryAsync<string>(getCountriesSql, getCountriesParameters);

            var getTripTypesSql = @"
                        SELECT DISTINCT Value 
                        FROM SourceAdditionalData
                        WHERE SourceId = @SourceId and Key = 'Type'";
            var getTripTypesParameters = new { SourceId = query.SourceId };

            var tripTypes = await connection.QueryAsync<string>(getTripTypesSql, getTripTypesParameters);

            var response = new GetHoryzontyFilterOptionsResponse
            {
                Countries = countries.OrderBy(x => x).ToList(),
                TripTypes = tripTypes.OrderBy(x => x).ToList()
            };
            return response;
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to get Horyzonty options from the database");
        }

        return new GetHoryzontyFilterOptionsResponse();
    }
}
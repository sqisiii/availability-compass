using System.Text;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;
using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.SearchSourcesQuery;

/// <summary>
/// Handles the search for data from sources
/// </summary>
/// <remarks>
/// This handler processes search requests for sources, applying filters on both direct source properties
/// and additional data associated with sources. It supports date range filtering and text search functionality.
/// The request and response classes are defined in a different class. See <see cref="SearchSourcesQuery"/> and <see cref="SearchSourcesResponse"/>.
/// </remarks>
public class SearchSourcesHandler : IRequestHandler<SearchRecords.Queries.SearchSources.SearchSourcesQuery, SearchSourcesResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SearchSourcesHandler(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<SearchSourcesResponse> Handle(SearchRecords.Queries.SearchSources.SearchSourcesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _dbConnectionFactory.Connect();
            var allResults = new List<SearchSourcesResponse.SourceDataItem>();

            const string baseSql = """
                                   SELECT
                                       s.SourceId, s.SeqNo, s.Title, s.Url, s.StartDate, s.EndDate, s.ChangeDate, sad.Key, sad.Value
                                   FROM Source s
                                   LEFT JOIN SourceAdditionalData sad
                                       ON s.SeqNo = sad.SourceSeqNo AND s.SourceId = sad.SourceId
                                   WHERE s.SourceId = @SourceId
                                   """;

            foreach (var querySource in query.Sources)
            {
                var sqlBuilder = new StringBuilder(baseSql);

                var parameters = new DynamicParameters();
                parameters.Add("SourceId", querySource.SourceId);

                var paramIndex = 0;
                foreach (var (filterKey, filterValues) in querySource.SelectedFiltersValues)
                {
                    if (filterValues.Count == 0)
                        continue;

                    if (filterKey.StartsWith("s."))
                    {
                        if (filterValues.Count == 1)
                        {
                            sqlBuilder.Append($" AND {filterKey} = @p{paramIndex}");
                            parameters.Add($"p{paramIndex++}", filterValues[0]);
                        }
                        else
                        {
                            sqlBuilder.Append($" AND {filterKey} IN (");
                            for (var i = 0; i < filterValues.Count; i++)
                            {
                                if (i > 0)
                                {
                                    sqlBuilder.Append(", ");
                                }

                                sqlBuilder.Append($"@p{paramIndex}");
                                parameters.Add($"p{paramIndex++}", filterValues[i]);
                            }

                            sqlBuilder.Append(')');
                        }
                    }
                    else if (filterKey.StartsWith("sad."))
                    {
                        var dataKey = filterKey[4..];

                        if (filterValues.Count == 1)
                        {
                            sqlBuilder.Append(" AND EXISTS (SELECT 1 FROM SourceAdditionalData sad2 WHERE ")
                                .Append("s.SeqNo = sad2.SourceSeqNo AND s.SourceId = sad2.SourceId ")
                                .Append($"AND sad2.Key = @pk{paramIndex} AND sad2.Value = @pv{paramIndex})");
                            parameters.Add($"pk{paramIndex}", dataKey);
                            parameters.Add($"pv{paramIndex}", filterValues[0]);
                        }
                        else
                        {
                            sqlBuilder.Append(" AND EXISTS (SELECT 1 FROM SourceAdditionalData sad2 WHERE ")
                                .Append("s.SeqNo = sad2.SourceSeqNo AND s.SourceId = sad2.SourceId ")
                                .Append($"AND sad2.Key = @pk{paramIndex} AND sad2.Value IN (");

                            parameters.Add($"pk{paramIndex}", dataKey);

                            for (var i = 0; i < filterValues.Count; i++)
                            {
                                if (i > 0)
                                {
                                    sqlBuilder.Append(", ");
                                }

                                sqlBuilder.Append($"@pv{paramIndex}_{i}");
                                parameters.Add($"pv{paramIndex}_{i}", filterValues[i]);
                            }

                            sqlBuilder.Append("))");
                        }

                        paramIndex++;
                    }
                    else
                        switch (filterKey)
                        {
                            case "search":
                                sqlBuilder.Append(" AND (s.Title LIKE @searchPhrase OR EXISTS (SELECT 1 FROM SourceAdditionalData sad2 WHERE ")
                                    .Append("s.SeqNo = sad2.SourceSeqNo AND s.SourceId = sad2.SourceId AND sad2.Value LIKE @searchPhrase))");
                                parameters.Add("searchPhrase", $"%{filterValues[0]}%");
                                break;
                            case "startDate":
                            case "endDate":
                            {
                                if (filterValues.Count == 1 && DateOnly.TryParse(filterValues[0], out var dateValue))
                                {
                                    if (filterKey == "startDate")
                                    {
                                        // Source's StartDate should be >= filter's startDate
                                        sqlBuilder.Append(" AND s.StartDate >= @dateStart");
                                        parameters.Add("dateStart", dateValue.ToString("yyyy-MM-dd"));
                                    }
                                    else // endDate
                                    {
                                        // Source's EndDate should be <= filter's endDate
                                        sqlBuilder.Append(" AND s.EndDate <= @dateEnd");
                                        parameters.Add("dateEnd", dateValue.ToString("yyyy-MM-dd"));
                                    }
                                }

                                break;
                            }
                        }
                }

                if (query.ReservedDates.Count > 0)
                {
                    // Create a StringBuilder for the CTE part only
                    var cteBuilder = new StringBuilder("WITH reserved_dates(date) AS (VALUES ");

                    for (var i = 0; i < query.ReservedDates.Count; i++)
                    {
                        if (i > 0) cteBuilder.Append(", ");
                        cteBuilder.Append($"(@resDate{i})");
                        parameters.Add($"resDate{i}", query.ReservedDates[i].ToString("yyyy-MM-dd"));
                    }

                    cteBuilder.Append(") ");

                    // Insert the CTE at the beginning of the entire query
                    sqlBuilder.Insert(0, cteBuilder.ToString());

                    // Add the NOT EXISTS condition separately
                    sqlBuilder.Append(" AND NOT EXISTS (")
                        .Append("SELECT 1 FROM reserved_dates WHERE ")
                        .Append("reserved_dates.date BETWEEN s.StartDate AND s.EndDate)");
                }

                // Add pagination
                // sqlBuilder.Append(" ORDER BY s.SourceId, s.SeqNo")
                //     .Append($" LIMIT {query.PageSize} OFFSET {(query.PageNumber - 1) * query.PageSize}");

                sqlBuilder.Append(" ORDER BY s.StartDate");
                var sql = sqlBuilder.ToString();

                Log.Debug("Executing search query: {Query}", sql);

                LogParametersToDebug(parameters);

                var sourceDictionary = new Dictionary<(int SeqNo, string SourceId), SearchSourcesResponse.SourceDataItem>();
                await connection.QueryAsync<SearchSourcesResponse.SourceDataItem, string, string, SearchSourcesResponse.SourceDataItem>(
                        sql,
                        map: (source, key, value) =>
                        {
                            var sourceKey = (source.SeqNo, source.SourceId);

                            if (!sourceDictionary.TryGetValue(sourceKey, out var existingSource))
                            {
                                existingSource = source;
                                sourceDictionary[sourceKey] = existingSource;
                            }

                            if (!string.IsNullOrEmpty(key))
                            {
                                existingSource.AdditionalData[key] = value;
                            }

                            return existingSource;
                        },
                        param: parameters,
                        splitOn: "Key,Value"
                    )
                    .ConfigureAwait(false);

                allResults.AddRange(sourceDictionary.Values);
            }

            return new SearchSourcesResponse(allResults);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while searching sources");
        }

        return new SearchSourcesResponse(new List<SearchSourcesResponse.SourceDataItem>()) { IsSuccess = false };
    }

    private static void LogParametersToDebug(DynamicParameters parameters)
    {
        var reservedDates = new List<string>();
        foreach (var param in parameters.ParameterNames)
        {
            if (param.StartsWith("resDate"))
            {
                reservedDates.Add(parameters.Get<object>(param)?.ToString() ?? "null");
            }
            else
            {
                Log.Debug("Query parameter: {Name} = {Value}", param, parameters.Get<object>(param));
            }
        }

        if (reservedDates.Count > 0)
        {
            Log.Debug("Query parameter for resDate% : {Dates}", string.Join(", ", reservedDates));
        }
    }
}
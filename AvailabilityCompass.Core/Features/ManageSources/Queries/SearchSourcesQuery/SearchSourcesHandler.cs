using System.Text;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;
using AvailabilityCompass.Core.Shared.Database;
using Dapper;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.SearchSourcesQuery;

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
            var allResults = new List<SourceDataItem>();

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

                // Add pagination
                // sqlBuilder.Append(" ORDER BY s.SourceId, s.SeqNo")
                //     .Append($" LIMIT {query.PageSize} OFFSET {(query.PageNumber - 1) * query.PageSize}");

                sqlBuilder.Append(" ORDER BY s.StartDate");
                var sql = sqlBuilder.ToString();

                Log.Debug("Executing search query: {Query}", sql);
                foreach (var param in parameters.ParameterNames)
                {
                    Log.Debug("Query parameter: {Name} = {Value}", param, parameters.Get<object>(param));
                }

                var sourceDictionary = new Dictionary<(int SeqNo, string SourceId), SourceDataItem>();
                await connection.QueryAsync<SourceDataItem, string, string, SourceDataItem>(
                        sql,
                        map: (source, key, value) =>
                        {
                            var sourceKey = (SeqNo: source.SeqNo, source.SourceId);

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

        return new SearchSourcesResponse(new List<SourceDataItem>()) { IsSuccess = false };
    }
}
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetFilterOptionsQuery;

public class GetFilterOptionsQuery : IRequest<GetFilterOptionsResponse>
{
    public GetFilterOptionsQuery(string sourceId, List<string> fieldNames)
    {
        SourceId = sourceId;
        FieldNames = fieldNames;
    }

    public string SourceId { get; }

    public List<string> FieldNames { get; }
}
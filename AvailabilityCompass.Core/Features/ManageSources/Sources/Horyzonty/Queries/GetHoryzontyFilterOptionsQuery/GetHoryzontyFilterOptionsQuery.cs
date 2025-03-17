using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources.Horyzonty.Queries.GetHoryzontyFilterOptionsQuery;

public class GetHoryzontyFilterOptionsQuery : IRequest<GetHoryzontyFilterOptionsResponse>
{
    public GetHoryzontyFilterOptionsQuery(string sourceId)
    {
        SourceId = sourceId;
    }

    public string SourceId { get; }
}
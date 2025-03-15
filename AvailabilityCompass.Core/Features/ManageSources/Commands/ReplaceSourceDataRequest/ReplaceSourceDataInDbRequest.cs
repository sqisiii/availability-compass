using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;

public class ReplaceSourceDataInDbRequest : IRequest<ReplaceSourceDataInDbResponse>
{
    public ReplaceSourceDataInDbRequest(IReadOnlyCollection<SourceDataItem> sources)
    {
        Sources = sources;
    }

    public IReadOnlyCollection<SourceDataItem> Sources { get; }
}
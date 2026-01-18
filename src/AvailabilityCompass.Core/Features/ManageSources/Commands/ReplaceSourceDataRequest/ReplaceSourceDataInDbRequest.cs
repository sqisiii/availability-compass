using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;

/// <summary>
/// MediatR request to replace source data in the database with new data.
/// </summary>
public class ReplaceSourceDataInDbRequest : IRequest<ReplaceSourceDataInDbResponse>
{
    public ReplaceSourceDataInDbRequest(IReadOnlyCollection<SourceDataItem> sources)
    {
        Sources = sources;
    }

    public IReadOnlyCollection<SourceDataItem> Sources { get; }
}
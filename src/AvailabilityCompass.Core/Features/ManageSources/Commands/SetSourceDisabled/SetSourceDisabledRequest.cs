using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Commands.SetSourceDisabled;

/// <summary>
/// MediatR request to set a source's disabled state.
/// </summary>
public class SetSourceDisabledRequest : IRequest<SetSourceDisabledResponse>
{
    public SetSourceDisabledRequest(string sourceId, bool isDisabled)
    {
        SourceId = sourceId;
        IsDisabled = isDisabled;
    }

    public string SourceId { get; }
    public bool IsDisabled { get; }
}

namespace AvailabilityCompass.Core.Features.ManageSources.Commands.SetSourceDisabled;

/// <summary>
/// Response from the set source disabled command indicating success or failure.
/// </summary>
public class SetSourceDisabledResponse
{
    public SetSourceDisabledResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}

using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateDateEntryRequest;

/// <summary>
/// Response from the update date entry command indicating success or failure.
/// </summary>
public class UpdateDateEntryInDbResponse : IProcessResult
{
    public UpdateDateEntryInDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}

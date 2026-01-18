using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddDateEntryRequest;

/// <summary>
/// Response from the add date entry command indicating success or failure.
/// </summary>
public class AddDateEntryToDbResponse : IProcessResult
{
    public AddDateEntryToDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}

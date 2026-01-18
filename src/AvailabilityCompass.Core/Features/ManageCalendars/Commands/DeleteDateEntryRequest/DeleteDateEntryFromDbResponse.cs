using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteDateEntryRequest;

/// <summary>
/// Response from the delete date entry command indicating success or failure.
/// </summary>
public class DeleteDateEntryFromDbResponse : IProcessResult
{
    public DeleteDateEntryFromDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}

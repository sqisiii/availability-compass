using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;

/// <summary>
/// Response from the add calendar command indicating success or failure.
/// </summary>
public class AddCalendarToDbResponse : IProcessResult
{
    public AddCalendarToDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}
using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;

/// <summary>
/// Response from the update calendar command indicating success or failure.
/// </summary>
public class UpdateCalendarInDbResponse : IProcessResult
{
    public UpdateCalendarInDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}
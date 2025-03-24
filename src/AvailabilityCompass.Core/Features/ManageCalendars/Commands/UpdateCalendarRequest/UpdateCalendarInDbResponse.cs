using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;

public class UpdateCalendarInDbResponse : IProcessResult
{
    public UpdateCalendarInDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}
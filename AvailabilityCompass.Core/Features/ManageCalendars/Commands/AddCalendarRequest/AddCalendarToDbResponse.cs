using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;

public class AddCalendarToDbResponse : IProcessResult
{
    public AddCalendarToDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; init; }
}
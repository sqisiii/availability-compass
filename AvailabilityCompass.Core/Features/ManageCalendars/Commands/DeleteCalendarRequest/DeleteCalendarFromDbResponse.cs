using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteCalendarRequest;

public class DeleteCalendarFromDbResponse : IProcessResult
{
    public DeleteCalendarFromDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}
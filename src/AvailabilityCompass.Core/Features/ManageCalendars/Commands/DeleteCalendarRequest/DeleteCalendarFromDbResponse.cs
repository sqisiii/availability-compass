using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteCalendarRequest;

/// <summary>
/// Response from the delete calendar command indicating success or failure.
/// </summary>
public class DeleteCalendarFromDbResponse : IProcessResult
{
    public DeleteCalendarFromDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}
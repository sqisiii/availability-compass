using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddRecurringDatesRequest;

public class AddRecurringDatesToDbResponse : IProcessResult
{
    public AddRecurringDatesToDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}
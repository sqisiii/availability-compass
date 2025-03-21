namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddRecurringDatesRequest;

public class AddRecurringDatesToDbResponse
{
    public AddRecurringDatesToDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}
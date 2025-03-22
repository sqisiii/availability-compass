using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateRecurringDateRequest;

public class UpdateRecurringDateInDbResponse : IProcessResult
{
    public UpdateRecurringDateInDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}
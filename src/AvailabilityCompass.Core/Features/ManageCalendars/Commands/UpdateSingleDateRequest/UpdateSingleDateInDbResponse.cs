using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateSingleDateRequest;

public class UpdateSingleDateInDbResponse : IProcessResult
{
    public UpdateSingleDateInDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}
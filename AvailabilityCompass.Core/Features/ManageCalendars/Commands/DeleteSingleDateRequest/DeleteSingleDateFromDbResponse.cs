using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteSingleDateRequest;

public class DeleteSingleDateFromDbResponse : IProcessResult
{
    public DeleteSingleDateFromDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}
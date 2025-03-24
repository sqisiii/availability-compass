using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddSingleDateRequest;

public class AddSingleDateToDbResponse : IProcessResult
{
    public AddSingleDateToDbResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}
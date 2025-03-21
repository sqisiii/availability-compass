namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddSingleDateRequest;

public class AddSingleDateToDbResponse
{
    public AddSingleDateToDbResponse(bool isSuccess)
    {
        this.isSuccess = isSuccess;
    }

    public bool isSuccess { get; }
}
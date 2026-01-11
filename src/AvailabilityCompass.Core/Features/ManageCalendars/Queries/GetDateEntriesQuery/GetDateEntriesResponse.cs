namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetDateEntriesQuery;

public class GetDateEntriesResponse
{
    public GetDateEntriesResponse(List<DateEntryDto> dateEntries, bool isSuccess)
    {
        DateEntries = dateEntries;
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
    public List<DateEntryDto> DateEntries { get; }
}

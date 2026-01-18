namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetDateEntriesQuery;

/// <summary>
/// Response containing the list of date entries for a calendar.
/// </summary>
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

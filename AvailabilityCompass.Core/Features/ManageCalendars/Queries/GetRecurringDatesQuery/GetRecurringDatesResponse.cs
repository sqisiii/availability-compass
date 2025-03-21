namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetRecurringDatesQuery;

public class GetRecurringDatesResponse
{
    public GetRecurringDatesResponse(List<RecurringDateDto> recurringDates, bool isSuccess)
    {
        IsSuccess = isSuccess;
        RecurringDates = recurringDates;
    }

    public bool IsSuccess { get; }
    public List<RecurringDateDto> RecurringDates { get; }
}
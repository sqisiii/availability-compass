namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetSingleDatesQuery;

public class GetSingleDatesResponse
{
    public GetSingleDatesResponse(List<SingleDateDto> singleDates, bool isSuccess)
    {
        IsSuccess = isSuccess;
        SingleDates = singleDates;
    }

    public bool IsSuccess { get; }

    public List<SingleDateDto> SingleDates { get; }
}
namespace AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;

public class SingleDateProcessor : IDateProcessor
{
    public List<CategorizedDate> Process(CalendarViewModel calendar)
    {
        List<CategorizedDate> result = [];

        foreach (var singleDate in calendar.SingleDates)
        {
            if (singleDate.Date is null)
            {
                continue;
            }

            result.Add(new CategorizedDate(
                singleDate.Date.Value.ToDateTime(TimeOnly.MinValue),
                CategorizedDateCategory.SingleDate,
                singleDate.Description));
        }

        return result;
    }
}
namespace AvailabilityCompass.Core.Features.ManageCalendars;

public class CalendarDto
{
    public CalendarDto()
    {
    }

    public CalendarDto(Guid id, string name, bool isOnly, DateTime changeDate)
    {
        Id = id;
        Name = name;
        IsOnly = isOnly;
        ChangeDate = changeDate;
    }

    public Guid Id { get; }

    public string Name { get; }

    public bool IsOnly { get; }

    public DateTime ChangeDate { get; }

    public List<SingleDateDto> SingleDates { get; } = [];

    public List<RecurringDateDto> RecurringDates { get; } = [];
}
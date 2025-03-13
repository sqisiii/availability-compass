namespace AvailabilityCompass.Core.Features.Search.Queries.GetCalendars;

public class GetCalendarsForFilteringDto
{
    public string Name { get; init; } = string.Empty;
    public Guid Id { get; init; } = Guid.Empty;
    public string Type { get; init; } = string.Empty;
}
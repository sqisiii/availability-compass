using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.GetCalendars;

/// <summary>
/// MediatR query to retrieve all calendars for use in search filtering.
/// </summary>
public class GetCalendarsForFilteringQuery : IRequest<GetCalendarsForFilteringResponse>
{
}
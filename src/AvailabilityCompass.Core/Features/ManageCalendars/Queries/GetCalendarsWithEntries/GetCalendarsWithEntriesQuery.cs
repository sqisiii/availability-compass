using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendarsWithEntries;

/// <summary>
/// Query to retrieve calendars with their date entries by ids.
/// </summary>
public record GetCalendarsWithEntriesQuery(List<Guid> CalendarIds) : IRequest<GetCalendarsWithEntriesResponse>;
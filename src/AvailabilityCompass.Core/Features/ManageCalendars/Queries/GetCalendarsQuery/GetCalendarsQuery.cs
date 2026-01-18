using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendarsQuery;

/// <summary>
/// MediatR query to retrieve all calendars with their date entries.
/// </summary>
public class GetCalendarsQuery : IRequest<GetCalendarsResponse>
{
}
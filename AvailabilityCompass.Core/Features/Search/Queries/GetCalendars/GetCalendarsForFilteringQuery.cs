using MediatR;

namespace AvailabilityCompass.Core.Features.Search.Queries.GetCalendars;

public class GetCalendarsForFilteringQuery : IRequest<IEnumerable<GetCalendarsForFilteringDto>>
{
}
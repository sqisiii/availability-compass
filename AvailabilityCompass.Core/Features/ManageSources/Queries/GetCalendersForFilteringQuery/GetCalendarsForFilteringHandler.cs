using AvailabilityCompass.Core.Features.Search.Queries.GetCalendars;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetCalendersForFilteringQuery;

public class GetCalendarsForFilteringHandler : IRequestHandler<GetCalendarsForFilteringQuery, IEnumerable<GetCalendarsForFilteringDto>>
{
    public Task<IEnumerable<GetCalendarsForFilteringDto>> Handle(GetCalendarsForFilteringQuery request, CancellationToken cancellationToken)
    {
        var calendars = new List<GetCalendarsForFilteringDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Type = "Inclusive",
                Name = "Calendar 1"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Type = "Exclusive",
                Name = "Calendar 2"
            }
        };
        return Task.FromResult(calendars.AsEnumerable());
    }
}
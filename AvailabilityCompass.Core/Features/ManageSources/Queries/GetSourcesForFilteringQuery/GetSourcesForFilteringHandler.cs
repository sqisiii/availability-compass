using AvailabilityCompass.Core.Features.Search.Queries.GetSources;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesForFilteringQuery;

public class GetSourcesForFilteringHandler : IRequestHandler<Search.Queries.GetSources.GetSourcesForFilteringQuery, GetSourcesForFilteringResponse>
{
    public Task<GetSourcesForFilteringResponse> Handle(Search.Queries.GetSources.GetSourcesForFilteringQuery request, CancellationToken cancellationToken)
    {
        var response = new GetSourcesForFilteringResponse();
        response.Sources.Add(new GetSourcesForFilteringResponse.Source { Name = "Horyzonty", ChangedAt = DateTime.Now.AddMinutes(-5) });
        response.Sources.Add(new GetSourcesForFilteringResponse.Source { Name = "Rowerzysta Podróżnik" });
        response.Sources.Add(new GetSourcesForFilteringResponse.Source { Name = "Itaka", ChangedAt = DateTime.Now.AddDays(-10), IsEnabled = false });
        response.Sources.Add(new GetSourcesForFilteringResponse.Source { Name = "Tui", ChangedAt = DateTime.Now.AddSeconds(-6), IsEnabled = false });
        response.Sources.Add(new GetSourcesForFilteringResponse.Source { Name = "Rainbow", ChangedAt = DateTime.Now.AddMonths(-2) });
        return Task.FromResult(response);
    }
}
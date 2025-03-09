using AvailabilityCompass.Core.Features.Search.Queries.GetSourcesQuery;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesQuery;

public class GetSourcesHandler : IRequestHandler<Search.Queries.GetSourcesQuery.GetSourcesQuery, GetSourcesResponse>
{
    public Task<GetSourcesResponse> Handle(Search.Queries.GetSourcesQuery.GetSourcesQuery request, CancellationToken cancellationToken)
    {
        var response = new GetSourcesResponse();
        response.Sources.Add(new GetSourcesResponse.Source { Name = "Horyzonty", ChangedAt = DateTime.Now.AddMinutes(-5) });
        response.Sources.Add(new GetSourcesResponse.Source { Name = "Rowerzysta Podróżnik", ChangedAt = DateTime.Now.AddMinutes(-15) });
        response.Sources.Add(new GetSourcesResponse.Source { Name = "Itaka", ChangedAt = DateTime.Now.AddDays(-10) });
        response.Sources.Add(new GetSourcesResponse.Source { Name = "Tui", ChangedAt = DateTime.Now.AddSeconds(-6) });
        response.Sources.Add(new GetSourcesResponse.Source { Name = "Rainbow", ChangedAt = DateTime.Now.AddMonths(-2) });
        return Task.FromResult(response);
    }
}
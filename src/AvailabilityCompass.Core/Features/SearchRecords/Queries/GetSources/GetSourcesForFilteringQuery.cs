using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.GetSources;

/// <summary>
/// MediatR query to retrieve all sources for use in search filtering.
/// </summary>
public class GetSourcesForFilteringQuery : IRequest<GetSourcesForFilteringResponse>
{
}
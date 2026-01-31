using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetDisabledSources;

/// <summary>
/// MediatR query to retrieve all disabled source IDs from the database.
/// </summary>
public class GetDisabledSourcesQuery : IRequest<GetDisabledSourcesResponse>
{
}

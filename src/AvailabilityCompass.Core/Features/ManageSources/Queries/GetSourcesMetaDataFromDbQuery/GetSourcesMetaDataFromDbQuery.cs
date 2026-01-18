using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;

/// <summary>
/// MediatR query to retrieve metadata for all sources from the database.
/// </summary>
public class GetSourcesMetaDataFromDbQuery : IRequest<IEnumerable<GetSourcesMetaDataFromDbDto>>
{
}
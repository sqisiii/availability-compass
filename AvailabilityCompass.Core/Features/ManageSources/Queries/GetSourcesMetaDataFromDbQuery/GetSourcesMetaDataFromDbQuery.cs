using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;

public class GetSourcesMetaDataFromDbQuery : IRequest<IEnumerable<GetSourcesMetaDataFromDbDto>>
{
}
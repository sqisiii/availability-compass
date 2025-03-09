using AvailabilityCompass.Core.Features.Search.Queries.GetSourcesQuery;

namespace AvailabilityCompass.Core.Features.Search;

public interface ISourceViewModelFactory
{
    SourceViewModel Create(GetSourcesResponse.Source getSourcesResponse);
}
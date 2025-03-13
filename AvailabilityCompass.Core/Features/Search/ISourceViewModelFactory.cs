using AvailabilityCompass.Core.Features.Search.Queries.GetSources;

namespace AvailabilityCompass.Core.Features.Search;

public interface ISourceViewModelFactory
{
    SourceViewModel Create(GetSourcesForFilteringResponse.Source getSourcesResponse);
}
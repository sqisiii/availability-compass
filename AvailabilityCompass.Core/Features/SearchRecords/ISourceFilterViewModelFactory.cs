using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetSources;

namespace AvailabilityCompass.Core.Features.SearchRecords;

public interface ISourceFilterViewModelFactory
{
    SourceFilterViewModel Create(GetSourcesForFilteringResponse.Source getSourcesResponse);
}
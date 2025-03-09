using AvailabilityCompass.Core.Features.Search.Queries.GetSources;

namespace AvailabilityCompass.Core.Features.Search;

public class SourceViewModelFactory : ISourceViewModelFactory
{
    public SourceViewModel Create(GetSourcesForFilteringResponse.Source getSourcesResponse)
    {
        return new SourceViewModel
        {
            IsActive = getSourcesResponse.IsEnabled,
            IsSelected = false,
            ChangeAt = getSourcesResponse.ChangedAt,
            Name = getSourcesResponse.Name
        };
    }
}
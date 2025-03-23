using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetSources;

namespace AvailabilityCompass.Core.Features.SearchRecords;

public class SourceFilterViewModelFactory : ISourceFilterViewModelFactory
{
    public SourceFilterViewModel Create(GetSourcesForFilteringResponse.Source getSourcesResponse)
    {
        return new SourceFilterViewModel
        {
            SourceId = getSourcesResponse.SourceId,
            IsActive = getSourcesResponse.IsEnabled,
            Language = getSourcesResponse.Language,
            IsSelected = false,
            ChangeAt = getSourcesResponse.ChangedAt,
            Name = getSourcesResponse.Name
        };
    }
}
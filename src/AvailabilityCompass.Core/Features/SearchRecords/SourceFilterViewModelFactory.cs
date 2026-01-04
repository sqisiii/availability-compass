using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetSources;
using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.Core.Features.SearchRecords;

public class SourceFilterViewModelFactory : ISourceFilterViewModelFactory
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public SourceFilterViewModelFactory(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }


    public SourceFilterViewModel Create(GetSourcesForFilteringResponse.Source getSourcesResponse)
    {
        return new SourceFilterViewModel(_dateTimeProvider)
        {
            SourceId = getSourcesResponse.SourceId,
            IsActive = getSourcesResponse.IsEnabled,
            Language = getSourcesResponse.Language,
            IsSelected = false,
            ChangeAt = getSourcesResponse.ChangedAt,
            Name = getSourcesResponse.Name,
            IconFileName = getSourcesResponse.IconFileName
        };
    }
}
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetSources;

namespace AvailabilityCompass.Core.Features.SearchRecords;

/// <summary>
/// Factory interface for creating instances of <see cref="SourceFilterViewModel"/>.
/// </summary>
public interface ISourceFilterViewModelFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="SourceFilterViewModel"/> based on the provided source response.
    /// </summary>
    /// <param name="getSourcesResponse">The source response containing the data needed to create the view model.</param>
    /// <returns>A new instance of <see cref="SourceFilterViewModel"/>.</returns>
    SourceFilterViewModel Create(GetSourcesForFilteringResponse.Source getSourcesResponse);
}

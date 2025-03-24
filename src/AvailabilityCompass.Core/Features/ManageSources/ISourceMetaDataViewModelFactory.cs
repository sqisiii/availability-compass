using AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;

namespace AvailabilityCompass.Core.Features.ManageSources;

/// <summary>
/// Factory interface for creating instances of <see cref="SourceMetaDataViewModel"/> from a collection of <see cref="GetSourcesMetaDataFromDbDto"/>.
/// </summary>
public interface ISourceMetaDataViewModelFactory
{
    /// <summary>
    /// Creates a collection of <see cref="SourceMetaDataViewModel"/> from the provided collection of <see cref="GetSourcesMetaDataFromDbDto"/>.
    /// </summary>
    /// <param name="sourcesMetaData">The collection of source metadata DTOs.</param>
    /// <returns>A collection of <see cref="SourceMetaDataViewModel"/>.</returns>
    IEnumerable<SourceMetaDataViewModel> Create(IEnumerable<GetSourcesMetaDataFromDbDto> sourcesMetaData);
}

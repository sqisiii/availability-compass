using AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;

namespace AvailabilityCompass.Core.Features.ManageSources;

public interface ISourceMetaDataViewModelFactory
{
    IEnumerable<SourceMetaDataViewModel> Create(IEnumerable<GetSourcesMetaDataFromDbDto> sourcesMetaData);
}
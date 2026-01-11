using AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;

namespace AvailabilityCompass.Core.Features.ManageSources;

public class SourceMetaDataViewModelFactory : ISourceMetaDataViewModelFactory
{
    private readonly ISourceStore _sourceStore;

    public SourceMetaDataViewModelFactory(ISourceStore sourceStore)
    {
        _sourceStore = sourceStore;
    }

    public IEnumerable<SourceMetaDataViewModel> Create(IEnumerable<GetSourcesMetaDataFromDbDto> sourcesMetaData)
    {
        var sourcesData = _sourceStore.GetSourceMetaData();

        var sourceMetaDataViewModels = new List<SourceMetaDataViewModel>();
        var getSourcesMetaDataFromDbDtos = sourcesMetaData.ToList();
        foreach (var sourceData in sourcesData.OrderBy(x => x.Name))
        {
            var sourceMetaData = getSourcesMetaDataFromDbDtos.FirstOrDefault(x => x.SourceId == sourceData.Id);
            var sourceMetaDataVm = new SourceMetaDataViewModel()
            {
                Name = sourceData.Name,
                Language = sourceData.Language,
                ChangedAt = sourceMetaData?.ChangedAt,
                SourceId = sourceData.Id,
                TripsCount = sourceMetaData?.TripsCount ?? 0,
                IsEnabled = sourceData.IsEnabled,
            };
            sourceMetaDataViewModels.Add(sourceMetaDataVm);
        }

        return sourceMetaDataViewModels;
    }
}
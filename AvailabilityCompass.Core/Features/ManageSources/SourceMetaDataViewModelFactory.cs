using AvailabilityCompass.Core.Features.ManageSources.Integrations;
using AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;

namespace AvailabilityCompass.Core.Features.ManageSources;

public class SourceMetaDataViewModelFactory : ISourceMetaDataViewModelFactory
{
    private readonly IIntegrationStore _integrationStore;

    public SourceMetaDataViewModelFactory(IIntegrationStore integrationStore)
    {
        _integrationStore = integrationStore;
    }

    public IEnumerable<SourceMetaDataViewModel> Create(IEnumerable<GetSourcesMetaDataFromDbDto> sourcesMetaData)
    {
        var integrationsData = _integrationStore.GetIntegrationsIdAndNames();

        var sourceMetaDataViewModels = new List<SourceMetaDataViewModel>();
        var getSourcesMetaDataFromDbDtos = sourcesMetaData.ToList();
        foreach (var integrationData in integrationsData)
        {
            var sourceMetaData = getSourcesMetaDataFromDbDtos.FirstOrDefault(x => x.IntegrationId == integrationData.Key);
            var sourceMetaDataVm = new SourceMetaDataViewModel()
            {
                Name = integrationData.Value,
                ChangedAt = sourceMetaData?.ChangedAt,
                IntegrationId = integrationData.Key,
                TripsCount = sourceMetaData?.TripsCount ?? 0,
            };
            sourceMetaDataViewModels.Add(sourceMetaDataVm);
        }

        return sourceMetaDataViewModels;
    }
}
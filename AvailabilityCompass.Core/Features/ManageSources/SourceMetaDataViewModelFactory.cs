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
        foreach (var integrationData in integrationsData.OrderBy(x => x.IntegrationName))
        {
            var sourceMetaData = getSourcesMetaDataFromDbDtos.FirstOrDefault(x => x.IntegrationId == integrationData.IntegrationId);
            var sourceMetaDataVm = new SourceMetaDataViewModel()
            {
                Name = integrationData.IntegrationName,
                ChangedAt = sourceMetaData?.ChangedAt,
                IntegrationId = integrationData.IntegrationId,
                TripsCount = sourceMetaData?.TripsCount ?? 0,
                IsEnabled = integrationData.IntegrationEnabled,
            };
            sourceMetaDataViewModels.Add(sourceMetaDataVm);
        }

        return sourceMetaDataViewModels;
    }
}
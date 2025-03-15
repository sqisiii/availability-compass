using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Features.ManageSources.Integrations;
using AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;
using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources;

public partial class ManageSourcesViewModel : ObservableValidator, IPageViewModel
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IIntegrationServiceFactory _integrationServiceFactory;
    private readonly IMediator _mediator;
    private readonly ISourceMetaDataViewModelFactory _sourceMetaDataViewModelFactory;

    public ManageSourcesViewModel(IIntegrationServiceFactory integrationServiceFactory, IMediator mediator, ISourceMetaDataViewModelFactory sourceMetaDataViewModelFactory)
    {
        _integrationServiceFactory = integrationServiceFactory;
        _mediator = mediator;
        _sourceMetaDataViewModelFactory = sourceMetaDataViewModelFactory;

        _ = LoadSourcesMetaDataAsync();
    }

    public ObservableCollection<SourceMetaDataViewModel> Sources { get; set; } = [];

    public bool IsActive { get; set; }
    public string Icon => "DatabaseCogOutline";
    public string Name => "Data";

    [RelayCommand]
    private async Task OnGetSources(string integrationId)
    {
        var integrationService = _integrationServiceFactory.GetService(integrationId);
        var sources = await integrationService.RefreshIntegrationDataAsync(_cancellationTokenSource.Token);
        await LoadSourcesMetaDataAsync();
    }

    private async Task LoadSourcesMetaDataAsync()
    {
        var sourcesMetaData = await _mediator.Send(new GetSourcesMetaDataFromDbQuery());
        Sources.Clear();
        var sourceViewModels = _sourceMetaDataViewModelFactory.Create(sourcesMetaData);
        foreach (var sourceViewModel in sourceViewModels)
        {
            Sources.Add(sourceViewModel);
        }
    }
}
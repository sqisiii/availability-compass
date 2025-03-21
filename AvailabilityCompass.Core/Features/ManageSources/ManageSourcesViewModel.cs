using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;
using AvailabilityCompass.Core.Features.ManageSources.Sources;
using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources;

public partial class ManageSourcesViewModel : ObservableValidator, IPageViewModel
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IMediator _mediator;
    private readonly ISourceMetaDataViewModelFactory _sourceMetaDataViewModelFactory;
    private readonly ISourceServiceFactory _sourceServiceFactory;

    public ManageSourcesViewModel(
        ISourceServiceFactory sourceServiceFactory,
        IMediator mediator,
        ISourceMetaDataViewModelFactory sourceMetaDataViewModelFactory)
    {
        _sourceServiceFactory = sourceServiceFactory;
        _mediator = mediator;
        _sourceMetaDataViewModelFactory = sourceMetaDataViewModelFactory;
    }

    public ObservableCollection<SourceMetaDataViewModel> Sources { get; set; } = [];

    public bool IsActive { get; set; }

    public string Icon => "DatabaseCogOutline";
    public string Name => "Data";

    public async Task LoadDataAsync(CancellationToken ct)
    {
        await LoadSourcesMetaDataAsync(ct);
    }

    [RelayCommand(CanExecute = nameof(CanRefreshSource))]
    private async Task OnRefreshSource(string sourceId, CancellationToken ct)
    {
        var sourceService = _sourceServiceFactory.GetService(sourceId);
        sourceService.RefreshProgressChanged += SourceServiceOnRefreshProgressChanged;
        var sources = await sourceService.RefreshSourceDataAsync(_cancellationTokenSource.Token);
        sourceService.RefreshProgressChanged -= SourceServiceOnRefreshProgressChanged;
        await LoadSourcesMetaDataAsync(ct);
    }

    public bool CanRefreshSource(string sourceId)
    {
        var source = Sources.FirstOrDefault(s => s.SourceId == sourceId);
        if (source == null)
        {
            return false;
        }

        return source.IsEnabled;
    }

    private void SourceServiceOnRefreshProgressChanged(object? sender, SourceRefreshProgressEventArgs e)
    {
        var source = Sources.FirstOrDefault(s => s.SourceId == e.SourceId);
        if (source == null)
        {
            return;
        }

        source.ProgressPercent = e.ProgressPercentage;
    }

    private async Task LoadSourcesMetaDataAsync(CancellationToken ct)
    {
        var sourcesMetaData = await _mediator.Send(new GetSourcesMetaDataFromDbQuery(), ct);
        Sources.Clear();
        var sourceViewModels = _sourceMetaDataViewModelFactory.Create(sourcesMetaData);
        foreach (var sourceViewModel in sourceViewModels)
        {
            Sources.Add(sourceViewModel);
        }
    }
}
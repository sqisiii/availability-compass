using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;
using AvailabilityCompass.Core.Features.ManageSources.Sources;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources;

/// <summary>
/// ViewModel for managing data sources in the application.
/// Provides functionality for viewing, refreshing, and monitoring source data.
/// </summary>
public partial class ManageSourcesViewModel : ObservableValidator, IPageViewModel, IDialogViewModel
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;
    private readonly IMediator _mediator;
    private readonly HashSet<string> _refreshingSourceIds = [];
    private readonly ISourceMetaDataViewModelFactory _sourceMetaDataViewModelFactory;
    private readonly ISourceServiceFactory _sourceServiceFactory;

    [ObservableProperty]
    private bool _isDialogOpen;

    public ManageSourcesViewModel(
        ISourceServiceFactory sourceServiceFactory,
        IMediator mediator,
        ISourceMetaDataViewModelFactory sourceMetaDataViewModelFactory,
        INavigationService<IDialogViewModel> dialogNavigationService)
    {
        _sourceServiceFactory = sourceServiceFactory;
        _mediator = mediator;
        _sourceMetaDataViewModelFactory = sourceMetaDataViewModelFactory;
        _dialogNavigationService = dialogNavigationService;
    }

    public ObservableCollection<SourceMetaDataViewModel> Sources { get; } = [];


    public bool IsActive { get; set; }

    public string Icon => "DatabaseCogOutline";
    public string Name => "Sources";

    public async Task LoadDataAsync(CancellationToken ct)
    {
        await LoadSourcesMetaDataAsync(ct);
    }

    [RelayCommand]
    private async Task OnRefreshAllSourcesAsync(CancellationToken ct)
    {
        var tasks = Sources
            .Where(source => source.IsEnabled)
            .Select(source => RefreshSourceData(source.SourceId, _cancellationTokenSource.Token))
            .ToList();

        await Task.WhenAll(tasks);
        await RefreshSourceMetaDataAsync(ct);
    }


    [RelayCommand(CanExecute = nameof(CanRefreshSource))]
    private async Task OnRefreshSource(string sourceId, CancellationToken ct)
    {
        await RefreshSourceData(sourceId, ct);
        await RefreshSourceMetaDataAsync(ct);
    }

    private async Task RefreshSourceMetaDataAsync(CancellationToken ct)
    {
        if (_refreshingSourceIds.Count > 0)
        {
            return;
        }

        await LoadSourcesMetaDataAsync(ct);
    }

    private async Task RefreshSourceData(string sourceId, CancellationToken ct)
    {
        if (!_refreshingSourceIds.Add(sourceId))
        {
            return;
        }

        RefreshSourceCommand.NotifyCanExecuteChanged();
        RefreshAllSourcesCommand.NotifyCanExecuteChanged();
        var sourceService = _sourceServiceFactory.GetService(sourceId);
        sourceService.RefreshProgressChanged += SourceServiceOnRefreshProgressChanged;
        await sourceService.RefreshSourceDataAsync(ct);
        sourceService.RefreshProgressChanged -= SourceServiceOnRefreshProgressChanged;
        _refreshingSourceIds.Remove(sourceId);
        RefreshSourceCommand.NotifyCanExecuteChanged();
        RefreshAllSourcesCommand.NotifyCanExecuteChanged();
    }

    public bool CanRefreshSource(string sourceId)
    {
        var source = Sources.FirstOrDefault(s => s.SourceId == sourceId);
        return source is not null && source.IsEnabled && !_refreshingSourceIds.Contains(sourceId);
    }

    private void SourceServiceOnRefreshProgressChanged(object? sender, SourceRefreshProgressEventArgs e)
    {
        var source = Sources.FirstOrDefault(s => s.SourceId == e.SourceId);
        if (source is null)
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

    [RelayCommand]
    private void OnClose()
    {
        IsDialogOpen = false;
        _dialogNavigationService.CloseView();
    }
}
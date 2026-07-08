using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Features.ManageSources.Commands.SetSourceDisabled;
using AvailabilityCompass.Core.Features.ManageSources.Queries.GetDisabledSources;
using AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;
using AvailabilityCompass.Core.Features.ManageSources.Sources;
using AvailabilityCompass.Core.Features.Tutorial;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guidely.Core;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources;

/// <summary>
/// ViewModel for managing data sources in the application.
/// Provides functionality for viewing, refreshing, and monitoring source data.
/// </summary>
public partial class ManageSourcesViewModel : ObservableValidator, IPageViewModel, IDialogViewModel
{
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;
    private readonly IMediator _mediator;
    private readonly HashSet<string> _refreshingSourceIds = [];
    private readonly ISourceMetaDataViewModelFactory _sourceMetaDataViewModelFactory;
    private readonly ISourceServiceFactory _sourceServiceFactory;
    private readonly TutorialViewModel<AvailabilityCompassContext, AppTutorialTrigger, AppTutorialGroup> _tutorialViewModel;

    [ObservableProperty]
    private bool _isDialogOpen;

    [ObservableProperty]
    private bool _isRefreshing;

    public ManageSourcesViewModel(
        ISourceServiceFactory sourceServiceFactory,
        IMediator mediator,
        ISourceMetaDataViewModelFactory sourceMetaDataViewModelFactory,
        INavigationService<IDialogViewModel> dialogNavigationService,
        TutorialViewModel<AvailabilityCompassContext, AppTutorialTrigger, AppTutorialGroup> tutorialViewModel)
    {
        _sourceServiceFactory = sourceServiceFactory;
        _mediator = mediator;
        _sourceMetaDataViewModelFactory = sourceMetaDataViewModelFactory;
        _dialogNavigationService = dialogNavigationService;
        _tutorialViewModel = tutorialViewModel;
    }

    public ObservableCollection<SourceMetaDataViewModel> Sources { get; } = [];


    public bool IsActive { get; set; }

    public string Icon => "DatabaseCogOutline";
    public string Name => "Sources";

    public async Task LoadDataAsync(CancellationToken ct)
    {
        await LoadSourcesMetaDataAsync(ct);
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task OnRefreshAllSourcesAsync(CancellationToken ct)
    {
        var tasks = Sources
            .Where(source => source.IsEnabled)
            .Select(source => RefreshSourceData(source.SourceId, ct))
            .ToList();

        await Task.WhenAll(tasks);
    }


    [RelayCommand(CanExecute = nameof(CanRefreshSource), IncludeCancelCommand = true)]
    private async Task OnRefreshSource(string sourceId, CancellationToken ct)
    {
        await RefreshSourceData(sourceId, ct);
    }

    /// <summary>
    /// Cancels any running refresh (single or all).
    /// </summary>
    [RelayCommand]
    private void OnCancelRefresh()
    {
        RefreshAllSourcesCancelCommand.Execute(null);
        RefreshSourceCancelCommand.Execute(null);
    }

    private async Task RefreshSourceData(string sourceId, CancellationToken ct)
    {
        if (!_refreshingSourceIds.Add(sourceId))
        {
            return;
        }

        _tutorialViewModel.FireTrigger(AppTutorialTrigger.SourceRefreshStarted);

        IsRefreshing = true;
        RefreshSourceCommand.NotifyCanExecuteChanged();
        RefreshAllSourcesCommand.NotifyCanExecuteChanged();
        var sourceService = _sourceServiceFactory.GetService(sourceId);
        sourceService.RefreshProgressChanged += SourceServiceOnRefreshProgressChanged;
        try
        {
            await sourceService.RefreshSourceDataAsync(ct);
            await UpdateSourceMetaDataAsync(sourceId, ct);

            _tutorialViewModel.FireTrigger(
                AppTutorialTrigger.SourceRefreshed,
                ctx => ctx with { HasRefreshedSource = true });
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Cancelled by the user; nothing to log.
        }
        catch (Exception ex)
        {
            // Includes HttpClient timeouts: TaskCanceledException without the command
            // token cancelled means the request timed out, not that the user cancelled.
            Log.Error(ex, "Refreshing source {SourceId} failed", sourceId);
        }
        finally
        {
            // Without this cleanup a failed refresh leaks the progress handler on the
            // singleton service and leaves the source's refresh button disabled forever.
            sourceService.RefreshProgressChanged -= SourceServiceOnRefreshProgressChanged;
            var source = Sources.FirstOrDefault(s => s.SourceId == sourceId);
            source?.ProgressPercent = 0;
            _refreshingSourceIds.Remove(sourceId);
            IsRefreshing = _refreshingSourceIds.Count > 0;
            RefreshSourceCommand.NotifyCanExecuteChanged();
            RefreshAllSourcesCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task UpdateSourceMetaDataAsync(string sourceId, CancellationToken ct)
    {
        var sourcesMetaData = await _mediator.Send(new GetSourcesMetaDataFromDbQuery(), ct);
        var sourceMetaData = sourcesMetaData.FirstOrDefault(s => s.SourceId == sourceId);

        var source = Sources.FirstOrDefault(s => s.SourceId == sourceId);
        if (source is not null && sourceMetaData is not null)
        {
            source.ChangedAt = sourceMetaData.ChangedAt;
            source.TripsCount = sourceMetaData.TripsCount;
            source.ProgressPercent = 0;
        }
    }

    public bool CanRefreshSource(string sourceId)
    {
        var source = Sources.FirstOrDefault(s => s.SourceId == sourceId);
        return source is not null && source.IsEnabled && !_refreshingSourceIds.Contains(sourceId);
    }

    private void SourceServiceOnRefreshProgressChanged(object? sender, SourceRefreshProgressEventArgs e)
    {
        var source = Sources.FirstOrDefault(s => s.SourceId == e.SourceId);
        source?.ProgressPercent = e.ProgressPercentage;
    }

    private async Task LoadSourcesMetaDataAsync(CancellationToken ct)
    {
        var sourcesMetaData = await _mediator.Send(new GetSourcesMetaDataFromDbQuery(), ct);
        var disabledSourcesResponse = await _mediator.Send(new GetDisabledSourcesQuery(), ct);
        Sources.Clear();
        var sourceViewModels = _sourceMetaDataViewModelFactory.Create(sourcesMetaData, disabledSourcesResponse.DisabledSourceIds);
        foreach (var sourceViewModel in sourceViewModels)
        {
            Sources.Add(sourceViewModel);
        }
    }

    [RelayCommand]
    private async Task OnToggleSourceEnabled(string sourceId, CancellationToken ct)
    {
        var source = Sources.FirstOrDefault(s => s.SourceId == sourceId);
        if (source is null)
        {
            return;
        }

        var newDisabledState = source.IsEnabled;
        var response = await _mediator.Send(new SetSourceDisabledRequest(sourceId, newDisabledState), ct);

        if (response.IsSuccess)
        {
            source.IsEnabled = !newDisabledState;
            RefreshSourceCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private void OnClose()
    {
        IsDialogOpen = false;
        _dialogNavigationService.CloseView();
    }
}
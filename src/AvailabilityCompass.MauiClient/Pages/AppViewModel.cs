using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageSettings;
using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.Core.Features.SearchRecords;
using AvailabilityCompass.Core.Features.Tutorial;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using AvailabilityCompass.MauiClient.Shared.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guidely.Core;

namespace AvailabilityCompass.MauiClient.Pages;

public partial class AppViewModel : ObservableObject, IDisposable
{
    private readonly INavigationStore<IDialogViewModel> _dialogNavigationStore;
    private readonly ManageCalendarsViewModel _manageCalendarsViewModel;
    private readonly ManageSourcesViewModel _manageSourcesViewModel;
    private readonly IThemeService _themeService;

    [NotifyPropertyChangedFor(nameof(ThemeIcon))]
    [ObservableProperty]
    private bool _isDarkTheme;

    public AppViewModel(
        INavigationStore<IDialogViewModel> dialogNavigationStore,
        IThemeService themeService,
        SearchViewModel searchViewModel,
        ManageSourcesViewModel manageSourcesViewModel,
        ManageCalendarsViewModel manageCalendarsViewModel,
        TutorialViewModel<AvailabilityCompassContext, AppTutorialTrigger, AppTutorialGroup> tutorialViewModel)
    {
        _dialogNavigationStore = dialogNavigationStore;
        _themeService = themeService;
        SearchViewModel = searchViewModel;
        _manageSourcesViewModel = manageSourcesViewModel;
        _manageCalendarsViewModel = manageCalendarsViewModel;
        TutorialViewModel = tutorialViewModel;

        _dialogNavigationStore.CurrentViewModelChanged += OnCurrentDialogViewModelChanged;
        _isDarkTheme = _themeService.IsDarkTheme;
    }

    public string ThemeIcon => IsDarkTheme ? "☀️" : "🌙";

    public SearchViewModel SearchViewModel { get; }

    public TutorialViewModel<AvailabilityCompassContext, AppTutorialTrigger, AppTutorialGroup> TutorialViewModel { get; }

    public IDialogViewModel? CurrentDialogViewModel => _dialogNavigationStore.CurrentViewModel;

    public bool IsDialogOpen => CurrentDialogViewModel?.IsDialogOpen ?? false;

    public void Dispose()
    {
        _dialogNavigationStore.CurrentViewModelChanged -= OnCurrentDialogViewModelChanged;
    }

    public async Task InitializeAsync()
    {
        IsDarkTheme = _themeService.IsDarkTheme;
        await SearchViewModel.LoadDataAsync(CancellationToken.None);
        await _manageSourcesViewModel.LoadDataAsync(CancellationToken.None);
        await _manageCalendarsViewModel.LoadDataAsync(CancellationToken.None);

        await TutorialViewModel.InitializeAsync(CancellationToken.None);
        UpdateTutorialContext();

        if (!HasAnySourceData())
        {
            await NavigationGate.GoToAsync("manage-sources");
        }
    }

    private bool HasAnySourceData()
    {
        return _manageSourcesViewModel.Sources.Any(source => source.TripsCount > 0);
    }

    private void UpdateTutorialContext()
    {
        var currentDialog = CurrentDialogViewModel switch
        {
            ManageSourcesViewModel => DialogType.Sources,
            ManageCalendarsViewModel => DialogType.Calendars,
            _ => DialogType.None
        };

        TutorialViewModel.UpdateContext(ctx => ctx with
        {
            HasSources = _manageSourcesViewModel.Sources.Count > 0,
            HasSourcesWithData = HasAnySourceData(),
            HasCalendars = _manageCalendarsViewModel.Calendars.Count > 0,
            CurrentDialog = currentDialog
        });
    }

    private void OnCurrentDialogViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentDialogViewModel));
        OnPropertyChanged(nameof(IsDialogOpen));

        var currentDialog = CurrentDialogViewModel switch
        {
            ManageSourcesViewModel => DialogType.Sources,
            ManageCalendarsViewModel => DialogType.Calendars,
            _ => DialogType.None
        };

        if (CurrentDialogViewModel is not null)
        {
            CurrentDialogViewModel.IsDialogOpen = true;
            OnPropertyChanged(nameof(IsDialogOpen));
        }

        TutorialViewModel.FireTrigger(
            AppTutorialTrigger.DialogChanged,
            ctx => ctx with
            {
                HasSources = _manageSourcesViewModel.Sources.Count > 0,
                HasSourcesWithData = HasAnySourceData(),
                HasCalendars = _manageCalendarsViewModel.Calendars.Count > 0,
                CurrentDialog = currentDialog
            });
    }

    [RelayCommand]
    private async Task OnToggleThemeAsync()
    {
        IsDarkTheme = !IsDarkTheme;
        await _themeService.SaveThemeAsync(IsDarkTheme);
    }

    [RelayCommand]
    private async Task OnOpenCalendarsAsync()
    {
        await NavigationGate.GoToAsync("manage-calendars");
    }

    [RelayCommand]
    private async Task OnOpenSourcesAsync()
    {
        await NavigationGate.GoToAsync("manage-sources");
    }
}
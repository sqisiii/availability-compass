using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageSettings;
using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.Core.Features.SearchRecords;
using AvailabilityCompass.Core.Features.Tutorial;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using AvailabilityCompass.WpfClient.Shared.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guidely.Core;

namespace AvailabilityCompass.WpfClient.Pages;

/// <summary>
/// Main view model for the application
/// </summary>
public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;
    private readonly INavigationStore<IDialogViewModel> _dialogNavigationStore;
    private readonly ManageCalendarsViewModel _manageCalendarsViewModel;
    private readonly ManageSourcesViewModel _manageSourcesViewModel;
    private readonly IThemeService _themeService;

    [NotifyPropertyChangedFor(nameof(ThemeIcon))]
    [ObservableProperty]
    private bool _isDarkTheme;

    private bool _isMaximized;

    public MainViewModel(
        INavigationStore<IDialogViewModel> dialogNavigationStore,
        INavigationService<IDialogViewModel> dialogNavigationService,
        IThemeService themeService,
        SearchViewModel searchViewModel,
        ManageSourcesViewModel manageSourcesViewModel,
        ManageCalendarsViewModel manageCalendarsViewModel,
        TutorialViewModel<AvailabilityCompassContext, AppTutorialTrigger, AppTutorialGroup> tutorialViewModel
    )
    {
        _dialogNavigationStore = dialogNavigationStore;
        _dialogNavigationService = dialogNavigationService;
        _themeService = themeService;
        SearchViewModel = searchViewModel;
        _manageSourcesViewModel = manageSourcesViewModel;
        _manageCalendarsViewModel = manageCalendarsViewModel;
        TutorialViewModel = tutorialViewModel;

        _dialogNavigationStore.CurrentViewModelChanged += OnCurrentDialogViewModelChanged;
        _isDarkTheme = _themeService.IsDarkTheme;
    }

    public string MaximizeIcon => _isMaximized ? FluentIcons.ChromeRestore : FluentIcons.ChromeMaximize;

    public string MaximizeToolTip => _isMaximized ? "Restore" : "Maximize";

    public string ThemeIcon => IsDarkTheme ? FluentIcons.Brightness : FluentIcons.ClearNight;

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

        // Initialize tutorial with current app state
        await TutorialViewModel.InitializeAsync(CancellationToken.None);
        UpdateTutorialContext();

        if (!HasAnySourceData())
        {
            _dialogNavigationService.NavigateTo(_manageSourcesViewModel);
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

        // Fire the dialog changed trigger with a context update
        TutorialViewModel.FireTrigger(
            AppTutorialTrigger.DialogChanged,
            ctx => ctx with
            {
                HasSources = _manageSourcesViewModel.Sources.Count > 0,
                HasSourcesWithData = HasAnySourceData(),
                HasCalendars = _manageCalendarsViewModel.Calendars.Count > 0,
                CurrentDialog = currentDialog
            }
        );
    }

    [RelayCommand]
    private void OnMaximizeButtonPressed()
    {
        _isMaximized = !_isMaximized;
        OnPropertyChanged(nameof(MaximizeIcon));
        OnPropertyChanged(nameof(MaximizeToolTip));
    }

    [RelayCommand]
    private async Task OnToggleThemeAsync()
    {
        IsDarkTheme = !IsDarkTheme;
        await _themeService.SaveThemeAsync(IsDarkTheme);
    }

    [RelayCommand]
    private void OnOpenCalendars()
    {
        _dialogNavigationService.NavigateTo(_manageCalendarsViewModel);
    }

    [RelayCommand]
    private void OnOpenSources()
    {
        _dialogNavigationService.NavigateTo(_manageSourcesViewModel);
    }

    [RelayCommand]
    private void OnCloseDialog()
    {
        _dialogNavigationService.CloseView();
    }
}
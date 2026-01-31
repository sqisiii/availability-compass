using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageSettings;
using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.Core.Features.SearchRecords;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using AvailabilityCompass.WpfClient.Shared.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
    private readonly SearchViewModel _searchViewModel;
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
        ManageCalendarsViewModel manageCalendarsViewModel
    )
    {
        _dialogNavigationStore = dialogNavigationStore;
        _dialogNavigationService = dialogNavigationService;
        _themeService = themeService;
        _searchViewModel = searchViewModel;
        _manageSourcesViewModel = manageSourcesViewModel;
        _manageCalendarsViewModel = manageCalendarsViewModel;

        _dialogNavigationStore.CurrentViewModelChanged += OnCurrentDialogViewModelChanged;
        _isDarkTheme = _themeService.IsDarkTheme;
    }

    public void Dispose()
    {
        _dialogNavigationStore.CurrentViewModelChanged -= OnCurrentDialogViewModelChanged;
    }

    public string MaximizeIcon => _isMaximized ? FluentIcons.ChromeRestore : FluentIcons.ChromeMaximize;

    public string MaximizeToolTip => _isMaximized ? "Restore" : "Maximize";

    public string ThemeIcon => IsDarkTheme ? FluentIcons.Brightness : FluentIcons.ClearNight;

    public SearchViewModel SearchViewModel => _searchViewModel;

    public IDialogViewModel? CurrentDialogViewModel => _dialogNavigationStore.CurrentViewModel;

    public bool IsDialogOpen => CurrentDialogViewModel?.IsDialogOpen ?? false;

    public async Task InitializeAsync()
    {
        IsDarkTheme = _themeService.IsDarkTheme;
        await _searchViewModel.LoadDataAsync(CancellationToken.None);
        await _manageSourcesViewModel.LoadDataAsync(CancellationToken.None);
        await _manageCalendarsViewModel.LoadDataAsync(CancellationToken.None);

        if (!HasAnySourceData())
        {
            _dialogNavigationService.NavigateTo(_manageSourcesViewModel);
        }
    }

    private bool HasAnySourceData()
    {
        return _manageSourcesViewModel.Sources.Any(source => source.TripsCount > 0);
    }

    private void OnCurrentDialogViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentDialogViewModel));
        OnPropertyChanged(nameof(IsDialogOpen));

        if (CurrentDialogViewModel is null)
        {
            return;
        }

        CurrentDialogViewModel.IsDialogOpen = true;
        OnPropertyChanged(nameof(IsDialogOpen));
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
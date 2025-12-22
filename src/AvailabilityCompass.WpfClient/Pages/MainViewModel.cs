using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageSettings;
using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.Core.Features.SearchRecords;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;

namespace AvailabilityCompass.WpfClient.Pages;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationStore<IDialogViewModel> _dialogNavigationStore;
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;
    private readonly IThemeService _themeService;
    private readonly SearchViewModel _searchViewModel;
    private readonly ManageSourcesViewModel _manageSourcesViewModel;
    private readonly ManageCalendarsViewModel _manageCalendarsViewModel;

    [NotifyPropertyChangedFor(nameof(MaximizeToolTip))]
    [ObservableProperty]
    private PackIcon _maximizeIcon = new() { Kind = PackIconKind.WindowMaximize };

    [NotifyPropertyChangedFor(nameof(ThemeIcon))]
    [ObservableProperty]
    private bool _isDarkTheme;

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

    public string MaximizeToolTip => MaximizeIcon.Kind == PackIconKind.WindowMaximize ? "Maximize" : "Restore";

    public PackIconKind ThemeIcon => IsDarkTheme ? PackIconKind.WeatherSunny : PackIconKind.WeatherNight;

    public SearchViewModel SearchViewModel => _searchViewModel;

    public IDialogViewModel? CurrentDialogViewModel => _dialogNavigationStore.CurrentViewModel;

    public bool IsDialogOpen => CurrentDialogViewModel?.IsDialogOpen ?? false;

    public async Task InitializeAsync()
    {
        IsDarkTheme = _themeService.IsDarkTheme;
        await _searchViewModel.LoadDataAsync(CancellationToken.None);
        await _manageSourcesViewModel.LoadDataAsync(CancellationToken.None);
        await _manageCalendarsViewModel.LoadDataAsync(CancellationToken.None);
    }

    private void OnCurrentDialogViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentDialogViewModel));
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
        MaximizeIcon = MaximizeIcon.Kind == PackIconKind.WindowMaximize
            ? new PackIcon { Kind = PackIconKind.WindowRestore }
            : new PackIcon { Kind = PackIconKind.WindowMaximize };
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
}

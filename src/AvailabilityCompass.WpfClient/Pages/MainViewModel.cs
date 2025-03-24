using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using AvailabilityCompass.WpfClient.Shared.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;

namespace AvailabilityCompass.WpfClient.Pages;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationStore<IDialogViewModel> _dialogNavigationStore;
    private readonly INavigationService<IPageViewModel> _mainNavigationService;
    private readonly INavigationStore<IPageViewModel> _mainNavigationStore;
    private readonly INavigationTabFactory _navigationTabFactory;

    [NotifyPropertyChangedFor(nameof(MaximizeToolTip))]
    [ObservableProperty]
    private PackIcon _maximizeIcon = new PackIcon { Kind = PackIconKind.WindowMaximize };

    public MainViewModel(
        INavigationTabFactory navigationTabFactory,
        INavigationStore<IPageViewModel> mainNavigationStore,
        INavigationStore<IDialogViewModel> dialogNavigationStore,
        INavigationService<IPageViewModel> mainNavigationService
    )
    {
        _navigationTabFactory = navigationTabFactory;
        _mainNavigationStore = mainNavigationStore;
        _dialogNavigationStore = dialogNavigationStore;
        _mainNavigationService = mainNavigationService;
        _mainNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        _dialogNavigationStore.CurrentViewModelChanged += OnCurrentDialogViewModelChanged;

        MainNavigationTabs = navigationTabFactory.CreateNavigationTabs();
    }

    public string MaximizeToolTip => MaximizeIcon.Kind == PackIconKind.WindowMaximize ? "Maximize" : "Restore";

    public IPageViewModel? CurrentViewModel => _mainNavigationStore.CurrentViewModel;

    public IDialogViewModel? CurrentDialogViewModel => _dialogNavigationStore.CurrentViewModel;
    public bool IsDialogOpen => CurrentDialogViewModel?.IsDialogOpen ?? false;

    public ObservableCollection<NavigationTabModel> MainNavigationTabs { get; }

    public void Initialize()
    {
        _mainNavigationService.NavigateTo(MainNavigationTabs.First().PageViewModel);
    }

    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
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
    private void OnTabChanged(NavigationTabModel? tab)
    {
        if (tab == null)
        {
            return;
        }

        _mainNavigationService.NavigateTo(tab.PageViewModel);
    }
}
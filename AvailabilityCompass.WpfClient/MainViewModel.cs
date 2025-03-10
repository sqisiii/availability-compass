using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.WpfClient.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;

namespace AvailabilityCompass.WpfClient;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly INavigationStore _navigationStore;

    [NotifyPropertyChangedFor(nameof(MaximizeToolTip))]
    [ObservableProperty]
    private PackIcon _maximizeIcon = new PackIcon { Kind = PackIconKind.Maximize };

    public MainViewModel(
        INavigationTabFactory navigationTabFactory,
        INavigationStore navigationStore,
        INavigationService navigationService)
    {
        _navigationStore = navigationStore;
        _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        _navigationService = navigationService;
        MainNavigationTabs = navigationTabFactory.CreateNavigationTabs();
        var pageViewModel = MainNavigationTabs.FirstOrDefault()?.PageViewModel;
        if (pageViewModel is not null)
        {
            _navigationService.NavigateTo(pageViewModel);
        }
    }

    public string MaximizeToolTip => MaximizeIcon.Kind == PackIconKind.WindowMaximize ? "Maximize" : "Restore";

    public IPageViewModel? CurrentViewModel => _navigationStore.CurrentViewModel;

    public ObservableCollection<NavigationTabModel> MainNavigationTabs { get; }

    [RelayCommand]
    private void OnMaximizeButtonPressed()
    {
        MaximizeIcon = MaximizeIcon.Kind == PackIconKind.WindowMaximize
            ? new PackIcon { Kind = PackIconKind.WindowRestore }
            : new PackIcon { Kind = PackIconKind.WindowMaximize };
    }

    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }

    [RelayCommand]
    private void OnTabChanged(NavigationTabModel? tab)
    {
        if (tab == null)
        {
            return;
        }

        _navigationService.NavigateTo(tab.PageViewModel);
    }
}
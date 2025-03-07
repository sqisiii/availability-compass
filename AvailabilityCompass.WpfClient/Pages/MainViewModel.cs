using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.WpfClient.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvailabilityCompass.WpfClient.Pages;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly INavigationStore _navigationStore;

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

    public IPageViewModel? CurrentViewModel => _navigationStore.CurrentViewModel;

    public ObservableCollection<NavigationTabModel> MainNavigationTabs { get; }

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
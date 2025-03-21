using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using AvailabilityCompass.WpfClient.Shared.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvailabilityCompass.WpfClient.Pages;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationStore<IDialogViewModel> _dialogNavigationStore;
    private readonly INavigationService<IPageViewModel> _mainNavigationService;
    private readonly INavigationStore<IPageViewModel> _mainNavigationStore;
    private readonly INavigationTabFactory _navigationTabFactory;

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
        if (CurrentDialogViewModel is not null)
        {
            CurrentDialogViewModel.IsDialogOpen = true;
            OnPropertyChanged(nameof(IsDialogOpen));
        }
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
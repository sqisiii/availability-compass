using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

public class NavigationService : INavigationService<IPageViewModel>
{
    private readonly INavigationStore<IPageViewModel> _navigationStore;

    public NavigationService(INavigationStore<IPageViewModel> navigationStore)
    {
        _navigationStore = navigationStore;
    }

    public void NavigateTo(IPageViewModel viewModel)
    {
        _navigationStore.CurrentViewModel = viewModel;
        _ = _navigationStore.CurrentViewModel.LoadDataAsync();
    }

    public void CloseView()
    {
        _navigationStore.CurrentViewModel = null;
    }
}
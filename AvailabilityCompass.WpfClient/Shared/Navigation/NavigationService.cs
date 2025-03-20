using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

public class NavigationService : INavigationService
{
    private readonly INavigationStore _navigationStore;

    public NavigationService(INavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
    }

    public void NavigateTo(IPageViewModel viewModel)
    {
        _navigationStore.CurrentViewModel = viewModel;
    }
}
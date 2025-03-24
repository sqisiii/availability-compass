using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

/// <summary>
/// Provides navigation services for navigating between views and dialogs.
/// </summary>
public class NavigationService : INavigationService<IPageViewModel>
{
    private readonly INavigationStore<IPageViewModel> _navigationStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="navigationStore">The navigation store that holds the current view model.</param>
    public NavigationService(INavigationStore<IPageViewModel> navigationStore)
    {
        _navigationStore = navigationStore;
    }

    /// <summary>
    /// Navigates to the specified view model.
    /// </summary>
    /// <param name="viewModel">The view model to navigate to.</param>
    public void NavigateTo(IPageViewModel viewModel)
    {
        _navigationStore.CurrentViewModel = viewModel;
        _ = _navigationStore.CurrentViewModel.LoadDataAsync(CancellationToken.None);
    }

    /// <summary>
    /// Closes the current view.
    /// </summary>
    public void CloseView()
    {
        _navigationStore.CurrentViewModel = null;
    }
}

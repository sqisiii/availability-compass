using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;

namespace AvailabilityCompass.MauiClient.Shared.Navigation;

public class NavigationStore : INavigationStore<IPageViewModel>
{
    private IPageViewModel? _currentViewModel;

    public IPageViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel?.IsActive = false;

            _currentViewModel = value;

            _currentViewModel?.IsActive = true;

            CurrentViewModelChanged?.Invoke();
        }
    }

    public event Action? CurrentViewModelChanged;
}
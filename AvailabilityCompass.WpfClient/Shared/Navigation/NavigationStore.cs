using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

public class NavigationStore : INavigationStore
{
    private IPageViewModel? _currentViewModel;

    public IPageViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (_currentViewModel != null)
            {
                _currentViewModel.IsActive = false;
            }

            _currentViewModel = value;

            if (_currentViewModel != null)
            {
                _currentViewModel.IsActive = true;
            }

            OnCurrentViewModelChanged();
        }
    }

    public event Action? CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

public class NavigationStore : INavigationStore<IPageViewModel>
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
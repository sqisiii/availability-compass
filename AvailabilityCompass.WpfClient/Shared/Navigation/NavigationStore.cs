using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

/// <summary>
/// Represents a store that holds the current view model for navigation purposes.
/// </summary>
public class NavigationStore : INavigationStore<IPageViewModel>
{
    private IPageViewModel? _currentViewModel;

    /// <summary>
    /// Gets or sets the current view model.
    /// </summary>
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

    /// <summary>
    /// Occurs when the current view model changes.
    /// </summary>
    public event Action? CurrentViewModelChanged;

    /// <summary>
    /// Raises the <see cref="CurrentViewModelChanged"/> event.
    /// </summary>
    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}

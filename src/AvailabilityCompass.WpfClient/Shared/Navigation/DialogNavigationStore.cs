using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

/// <summary>
/// A store that manages the current dialog view model and handles its activation state.
/// </summary>
public class DialogNavigationStore : INavigationStore<IDialogViewModel>
{
    private IDialogViewModel? _currentViewModel;

    /// <summary>
    /// Gets or sets the current dialog view model.
    /// When the current view model is set, it updates the IsActive property of the previous and new view models.
    /// </summary>
    public IDialogViewModel? CurrentViewModel
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
    /// Event that is triggered when the current view model changes.
    /// </summary>
    public event Action? CurrentViewModelChanged;

    /// <summary>
    /// Invokes the CurrentViewModelChanged event.
    /// </summary>
    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}

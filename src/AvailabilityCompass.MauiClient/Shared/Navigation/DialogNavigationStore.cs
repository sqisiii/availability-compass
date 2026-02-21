using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;

namespace AvailabilityCompass.MauiClient.Shared.Navigation;

public class DialogNavigationStore : INavigationStore<IDialogViewModel>
{
    private IDialogViewModel? _currentViewModel;

    public IDialogViewModel? CurrentViewModel
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
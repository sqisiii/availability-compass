using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using MaterialDesignThemes.Wpf;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

/// <summary>
/// Provides navigation services for dialogs.
/// </summary>
public class DialogNavigationService : INavigationService<IDialogViewModel>
{
    private readonly INavigationStore<IDialogViewModel> _dialogNavigationStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogNavigationService"/> class.
    /// </summary>
    /// <param name="dialogNavigationStore">The navigation store that holds the current dialog view model.</param>
    public DialogNavigationService(INavigationStore<IDialogViewModel> dialogNavigationStore)
    {
        _dialogNavigationStore = dialogNavigationStore;
    }

    /// <summary>
    /// Navigates to the specified dialog view model.
    /// </summary>
    /// <param name="viewModel">The view model to navigate to.</param>
    public void NavigateTo(IDialogViewModel viewModel)
    {
        _dialogNavigationStore.CurrentViewModel = viewModel;
    }

    /// <summary>
    /// Closes the current dialog view.
    /// </summary>
    public void CloseView()
    {
        if (_dialogNavigationStore.CurrentViewModel == null)
        {
            return;
        }

        DialogHost.CloseDialogCommand.Execute(null, null);
        _dialogNavigationStore.CurrentViewModel = null;
    }
}

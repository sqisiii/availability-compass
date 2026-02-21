using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;

namespace AvailabilityCompass.MauiClient.Shared.Navigation;

public class DialogNavigationService : INavigationService<IDialogViewModel>
{
    private readonly INavigationStore<IDialogViewModel> _dialogNavigationStore;

    public DialogNavigationService(INavigationStore<IDialogViewModel> dialogNavigationStore)
    {
        _dialogNavigationStore = dialogNavigationStore;
    }

    public void NavigateTo(IDialogViewModel viewModel)
    {
        _dialogNavigationStore.CurrentViewModel = viewModel;
    }

    public void CloseView()
    {
        if (_dialogNavigationStore.CurrentViewModel == null)
            return;

        _dialogNavigationStore.CurrentViewModel = null;
    }
}
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using MaterialDesignThemes.Wpf;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

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
        {
            return;
        }

        DialogHost.CloseDialogCommand.Execute(null, null);
        _dialogNavigationStore.CurrentViewModel = null;
    }
}
namespace AvailabilityCompass.Core.Shared.Navigation;

public interface INavigationService<in T>
{
    void NavigateTo(T viewModel);

    void CloseView();
}
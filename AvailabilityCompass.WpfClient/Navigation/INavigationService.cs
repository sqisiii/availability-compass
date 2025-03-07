using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.WpfClient.Navigation;

public interface INavigationService
{
    void NavigateTo(IPageViewModel viewModel);
}
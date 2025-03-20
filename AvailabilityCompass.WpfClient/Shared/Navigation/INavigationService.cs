using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

public interface INavigationService
{
    void NavigateTo(IPageViewModel viewModel);
}
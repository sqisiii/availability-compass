using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

public interface INavigationStore
{
    IPageViewModel? CurrentViewModel { get; set; }
    event Action? CurrentViewModelChanged;
}
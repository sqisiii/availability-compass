using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.WpfClient.Navigation;

public interface INavigationStore
{
    IPageViewModel? CurrentViewModel { get; set; }
    event Action? CurrentViewModelChanged;
}
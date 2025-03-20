using System.Collections.ObjectModel;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

public interface INavigationTabFactory
{
    ObservableCollection<NavigationTabModel> CreateNavigationTabs();
}
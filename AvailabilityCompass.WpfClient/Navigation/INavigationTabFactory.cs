using System.Collections.ObjectModel;

namespace AvailabilityCompass.WpfClient.Navigation;

public interface INavigationTabFactory
{
    ObservableCollection<NavigationTabModel> CreateNavigationTabs();
}
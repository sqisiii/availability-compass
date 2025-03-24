using System.Collections.ObjectModel;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

/// <summary>
/// Defines a factory for creating navigation tabs.
/// </summary>
public interface INavigationTabFactory
{
    /// <summary>
    /// Creates a collection of navigation tabs.
    /// </summary>
    /// <returns>An observable collection of <see cref="NavigationTabModel"/>.</returns>
    ObservableCollection<NavigationTabModel> CreateNavigationTabs();
}

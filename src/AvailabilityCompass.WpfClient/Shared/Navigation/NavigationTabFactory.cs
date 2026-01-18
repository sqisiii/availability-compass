using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.Core.Features.SearchRecords;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

/// <summary>
/// Factory for creating navigation tabs from registered page view models.
/// </summary>
public class NavigationTabFactory : INavigationTabFactory
{
    private readonly ManageCalendarsViewModel _manageCalendarsViewModel;
    private readonly ManageSourcesViewModel _manageSourcesViewModel;
    private readonly SearchViewModel _searchViewModel;

    public NavigationTabFactory(
        SearchViewModel searchViewModel,
        ManageCalendarsViewModel manageCalendarsViewModel,
        ManageSourcesViewModel manageSourcesViewModel)
    {
        _manageCalendarsViewModel = manageCalendarsViewModel;
        _manageSourcesViewModel = manageSourcesViewModel;
        _searchViewModel = searchViewModel;
    }

    /// <inheritdoc />
    public ObservableCollection<NavigationTabModel> CreateNavigationTabs()
    {
        return
        [
            new NavigationTabModel(_searchViewModel),
            new NavigationTabModel(_manageCalendarsViewModel),
            new NavigationTabModel(_manageSourcesViewModel)
        ];
    }
}
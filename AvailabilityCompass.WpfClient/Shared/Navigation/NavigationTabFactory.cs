using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageSettings;
using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.Core.Features.SearchRecords;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

public class NavigationTabFactory : INavigationTabFactory
{
    private readonly ManageCalendarsViewModel _manageCalendarsViewModel;
    private readonly ManageSettingsViewModel _manageSettingsViewModel;
    private readonly ManageSourcesViewModel _manageSourcesViewModel;
    private readonly SearchViewModel _searchViewModel;

    public NavigationTabFactory(
        SearchViewModel searchViewModel,
        ManageCalendarsViewModel manageCalendarsViewModel,
        ManageSettingsViewModel manageSettingsViewModel,
        ManageSourcesViewModel manageSourcesViewModel)
    {
        _manageCalendarsViewModel = manageCalendarsViewModel;
        _manageSettingsViewModel = manageSettingsViewModel;
        _manageSourcesViewModel = manageSourcesViewModel;
        _searchViewModel = searchViewModel;
    }

    public ObservableCollection<NavigationTabModel> CreateNavigationTabs()
    {
        return
        [
            new NavigationTabModel(_searchViewModel),
            new NavigationTabModel(_manageCalendarsViewModel),
            new NavigationTabModel(_manageSourcesViewModel),
            new NavigationTabModel(_manageSettingsViewModel)
        ];
    }
}
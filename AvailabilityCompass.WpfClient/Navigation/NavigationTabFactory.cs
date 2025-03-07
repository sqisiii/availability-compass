using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageSettings;
using AvailabilityCompass.Core.Features.SelectCriteria;

namespace AvailabilityCompass.WpfClient.Navigation;

public class NavigationTabFactory : INavigationTabFactory
{
    private readonly ManageCalendarsViewModel _manageCalendarsViewModel;
    private readonly ManageSettingsViewModel _manageSettingsViewModel;
    private readonly SelectCriteriaViewModel _selectCriteriaViewModel;

    public NavigationTabFactory(
        SelectCriteriaViewModel selectCriteriaViewModel,
        ManageCalendarsViewModel manageCalendarsViewModel,
        ManageSettingsViewModel manageSettingsViewModel)
    {
        _selectCriteriaViewModel = selectCriteriaViewModel;
        _manageCalendarsViewModel = manageCalendarsViewModel;
        _manageSettingsViewModel = manageSettingsViewModel;
    }

    public ObservableCollection<NavigationTabModel> CreateNavigationTabs()
    {
        return
        [
            new NavigationTabModel(_selectCriteriaViewModel),
            new NavigationTabModel(_manageCalendarsViewModel),
            new NavigationTabModel(_manageSettingsViewModel),
        ];
    }
}
using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.WpfClient.Shared.Navigation;

/// <summary>
/// Represents a navigation tab with its associated view model, name, and icon.
/// </summary>
public class NavigationTabModel
{
    public NavigationTabModel(IPageViewModel pageViewModel)
    {
        PageViewModel = pageViewModel;
        Name = pageViewModel.Name;
        Icon = pageViewModel.Icon;
    }

    public string Name { get; }
    public string Icon { get; }
    public IPageViewModel PageViewModel { get; }
}
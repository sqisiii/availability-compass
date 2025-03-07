using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.WpfClient.Navigation;

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
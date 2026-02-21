using AvailabilityCompass.MauiClient.Pages.ManageCalendars;
using AvailabilityCompass.MauiClient.Pages.ManageSources;
using AvailabilityCompass.MauiClient.Pages.SearchRecords;

namespace AvailabilityCompass.MauiClient;

public class AppShell : Shell
{
    public AppShell()
    {
        var tabBar = new TabBar();

        tabBar.Items.Add(new ShellContent
        {
            Title = "Search",
            ContentTemplate = new DataTemplate(typeof(SearchPage)),
            Route = "search"
        });

        Items.Add(tabBar);

        // Register modal routes
        Routing.RegisterRoute("manage-sources", typeof(ManageSourcesPage));
        Routing.RegisterRoute("manage-calendars", typeof(ManageCalendarsPage));
    }
}
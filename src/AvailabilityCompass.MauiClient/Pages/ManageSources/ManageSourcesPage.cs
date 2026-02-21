using AvailabilityCompass.Core.Features.ManageSources;

namespace AvailabilityCompass.MauiClient.Pages.ManageSources;

public class ManageSourcesPage : ContentPage
{
    public ManageSourcesPage(ManageSourcesViewModel vm)
    {
        BindingContext = vm;
        Title = "Manage Sources";

        Content = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Label { Text = "Manage Sources", FontSize = 28, HorizontalOptions = LayoutOptions.Center },
                new Label { Text = "Coming in Phase 4", FontSize = 18, HorizontalOptions = LayoutOptions.Center, Opacity = 0.6 }
            }
        };
    }
}
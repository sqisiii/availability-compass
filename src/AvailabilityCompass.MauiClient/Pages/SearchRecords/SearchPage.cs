using AvailabilityCompass.Core.Features.SearchRecords;

namespace AvailabilityCompass.MauiClient.Pages.SearchRecords;

public class SearchPage : ContentPage
{
    public SearchPage(SearchViewModel vm)
    {
        BindingContext = vm;
        Title = "Search";

        Content = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Label { Text = "Availability Compass", FontSize = 32, HorizontalOptions = LayoutOptions.Center },
                new Label { Text = "Search - Coming in Phase 5", FontSize = 18, HorizontalOptions = LayoutOptions.Center, Opacity = 0.6 }
            }
        };
    }
}
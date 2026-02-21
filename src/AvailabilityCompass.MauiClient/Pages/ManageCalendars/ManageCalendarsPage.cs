using AvailabilityCompass.Core.Features.ManageCalendars;

namespace AvailabilityCompass.MauiClient.Pages.ManageCalendars;

public class ManageCalendarsPage : ContentPage
{
    public ManageCalendarsPage(ManageCalendarsViewModel vm)
    {
        BindingContext = vm;
        Title = "Manage Calendars";

        Content = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Label { Text = "Manage Calendars", FontSize = 28, HorizontalOptions = LayoutOptions.Center },
                new Label { Text = "Coming in Phase 7", FontSize = 18, HorizontalOptions = LayoutOptions.Center, Opacity = 0.6 }
            }
        };
    }
}
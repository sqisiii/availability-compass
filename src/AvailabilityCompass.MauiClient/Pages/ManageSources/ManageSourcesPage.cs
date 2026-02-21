using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.MauiClient.Shared.Converters;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;

namespace AvailabilityCompass.MauiClient.Pages.ManageSources;

public class ManageSourcesPage : ContentPage
{
    private readonly ManageSourcesViewModel _vm;

    public ManageSourcesPage(ManageSourcesViewModel vm)
    {
        _vm = vm;
        BindingContext = vm;
        Title = "Manage Sources";

        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Refresh All",
            Command = vm.RefreshAllSourcesCommand
        });

        Content = new CollectionView
            {
                ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical)
                {
                    HorizontalItemSpacing = 12,
                    VerticalItemSpacing = 12
                },
                ItemTemplate = new DataTemplate(BuildSourceCard),
                SelectionMode = SelectionMode.None
            }
            .Bind(ItemsView.ItemsSourceProperty, nameof(ManageSourcesViewModel.Sources))
            .Margins(16, 16, 16, 16);
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _ = _vm.LoadDataAsync(CancellationToken.None);
    }

    private View BuildSourceCard()
    {
        var boolToOpacity = new BoolToOpacityConverter();
        var inverseBool = new InverseBoolConverter();
        var percentConverter = new DoubleToPercentageConverter();

        var card = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Stroke = Colors.LightGray,
            StrokeThickness = 1,
            Padding = 16,
            MinimumHeightRequest = 160,
            Content = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto)
                },
                Children =
                {
                    // Row 0: Header
                    BuildHeaderRow(),

                    // Row 1: Stats
                    BuildStatsSection().Row(1),

                    // Row 2: Actions
                    BuildActionsSection(inverseBool, percentConverter).Row(2)
                }
            }
        };

        card.Bind(OpacityProperty, nameof(SourceMetaDataViewModel.IsEnabled),
            converter: boolToOpacity);

        return card;
    }

    private static View BuildHeaderRow()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto)
            },
            Children =
            {
                new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16,
                        VerticalTextAlignment = TextAlignment.Center
                    }
                    .Bind(Label.TextProperty, nameof(SourceMetaDataViewModel.Name))
                    .Column(0),

                new Label
                    {
                        FontSize = 12,
                        Opacity = 0.6,
                        VerticalTextAlignment = TextAlignment.Center
                    }
                    .Bind(Label.TextProperty, nameof(SourceMetaDataViewModel.Language))
                    .Column(1)
                    .Margins(0, 0, 8),

                new Label
                    {
                        FontSize = 18,
                        VerticalTextAlignment = TextAlignment.Center
                    }
                    .Bind(Label.TextProperty, nameof(SourceMetaDataViewModel.IsEnabled),
                        converter: new FuncConverter<bool, string>(isEnabled => isEnabled ? "👁" : "🚫"))
                    .Column(2)
            }
        };
    }

    private static View BuildStatsSection()
    {
        return new VerticalStackLayout
        {
            Spacing = 6,
            Margin = new Thickness(0, 12, 0, 0),
            Children =
            {
                new HorizontalStackLayout
                {
                    Spacing = 6,
                    Children =
                    {
                        new Label { Text = "📊", FontSize = 14 },
                        new Label { FontSize = 13, Opacity = 0.7 }
                            .Bind(Label.TextProperty, nameof(SourceMetaDataViewModel.TripsCount),
                                converter: new FuncConverter<int, string>(count => $"{count:N0} records"))
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 6,
                    Children =
                    {
                        new Label { Text = "🕐", FontSize = 14 },
                        new Label { FontSize = 13, Opacity = 0.7 }
                            .Bind(Label.TextProperty, nameof(SourceMetaDataViewModel.ChangedAt),
                                converter: new FuncConverter<DateTime?, string>(dt =>
                                    dt?.ToString("yyyy-MM-dd HH:mm") ?? "Never updated"))
                    }
                }
            }
        };
    }

    private View BuildActionsSection(InverseBoolConverter inverseBool, DoubleToPercentageConverter percentConverter)
    {
        return new Grid
        {
            Margin = new Thickness(0, 16, 0, 0),
            Children =
            {
                // Progress bar (visible when refreshing)
                new Grid
                    {
                        Children =
                        {
                            new ProgressBar { HeightRequest = 28 }
                                .Bind(ProgressBar.ProgressProperty, nameof(SourceMetaDataViewModel.ProgressPercent),
                                    converter: new FuncConverter<double, double>(p => p / 100.0)),
                            new Label
                                {
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    VerticalTextAlignment = TextAlignment.Center,
                                    FontSize = 12,
                                    FontAttributes = FontAttributes.Bold
                                }
                                .Bind(Label.TextProperty, nameof(SourceMetaDataViewModel.ProgressPercent),
                                    converter: percentConverter)
                        }
                    }
                    .Bind(IsVisibleProperty, nameof(SourceMetaDataViewModel.ShowProgress)),

                // Refresh button (visible when idle)
                new Button { Text = "Refresh", Command = _vm.RefreshSourceCommand }
                    .Bind(Button.CommandParameterProperty, nameof(SourceMetaDataViewModel.SourceId))
                    .Bind(IsVisibleProperty, nameof(SourceMetaDataViewModel.ShowProgress),
                        converter: inverseBool)
            }
        };
    }
}
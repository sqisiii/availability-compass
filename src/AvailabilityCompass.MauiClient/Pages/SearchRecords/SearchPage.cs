using AvailabilityCompass.Core.Features.SearchRecords;
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.MauiClient.Shared.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace AvailabilityCompass.MauiClient.Pages.SearchRecords;

public class SearchPage : ContentPage
{
    private readonly InverseBoolConverter _inverseBool = new();
    private readonly SearchViewModel _vm;

    public SearchPage(SearchViewModel vm)
    {
        _vm = vm;
        BindingContext = vm;
        Title = "Availability Compass";

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Sources",
            Command = new AsyncRelayCommand(() => Shell.Current.GoToAsync("manage-sources"))
        });
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Calendars",
            Command = new AsyncRelayCommand(() => Shell.Current.GoToAsync("manage-calendars"))
        });

        Content = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            },
            Children =
            {
                BuildFilterArea().Row(0),
                BuildResultsArea().Row(1)
            }
        };
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _ = _vm.LoadDataAsync(CancellationToken.None);
    }

    private View BuildSectionHeader(string title, string summaryProperty, string expandedProperty)
    {
        var header = new Grid
        {
            Padding = new Thickness(16, 12),
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            Children =
            {
                new Label
                    {
                        Text = title, FontAttributes = FontAttributes.Bold, FontSize = 14, VerticalTextAlignment = TextAlignment.Center
                    }
                    .Column(0),
                new Label
                    {
                        FontSize = 12, Opacity = 0.6, VerticalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.TailTruncation
                    }
                    .Bind(Label.TextProperty, summaryProperty)
                    .Column(1)
                    .Margins(8),
                new Label { FontSize = 14, VerticalTextAlignment = TextAlignment.Center }
                    .Bind(Label.TextProperty, expandedProperty,
                        converter: new FuncConverter<bool, string>(expanded => expanded ? "▲" : "▼"))
                    .Column(2)
            }
        };

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (_, _) =>
        {
            var prop = typeof(SearchViewModel).GetProperty(expandedProperty);
            if (prop is null) return;
            var current = (bool)(prop.GetValue(_vm) ?? false);
            prop.SetValue(_vm, !current);
        };
        header.GestureRecognizers.Add(tapGesture);

        return header;
    }

    private View BuildFilterArea()
    {
        return new VerticalStackLayout
        {
            Spacing = 0,
            BackgroundColor = Color.FromArgb("#F8F8F8"),
            Children =
            {
                BuildCalendarsSection(),
                new BoxView { HeightRequest = 1, Color = Colors.LightGray },
                BuildSourcesSection(),
                new BoxView { HeightRequest = 1, Color = Colors.LightGray },
                BuildFiltersSection(),
                new BoxView { HeightRequest = 1, Color = Colors.LightGray },
                BuildSearchButton()
            }
        };
    }

    private View BuildCalendarsSection()
    {
        var header = BuildSectionHeader("Calendars",
            nameof(SearchViewModel.CalendarsSummary),
            nameof(SearchViewModel.IsCalendarsSectionExpanded));

        var content = new VerticalStackLayout
            {
                Padding = new Thickness(16, 0, 16, 12),
                Spacing = 8,
                Children =
                {
                    BuildCalendarChips(),
                    BuildCalendarModeChips()
                }
            }
            .Bind(IsVisibleProperty, nameof(SearchViewModel.IsCalendarsSectionExpanded));

        return new VerticalStackLayout
        {
            Children = { header, content }
        };
    }

    private View BuildCalendarChips()
    {
        var layout = new FlexLayout
        {
            Wrap = FlexWrap.Wrap,
            JustifyContent = FlexJustify.Start,
            AlignItems = FlexAlignItems.Center
        };

        layout.SetBinding(BindableLayout.ItemsSourceProperty, nameof(SearchViewModel.Calendars));

        BindableLayout.SetItemTemplate(layout, new DataTemplate(() =>
        {
            var chip = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 16 },
                Padding = new Thickness(12, 6),
                Margin = new Thickness(0, 0, 8, 8),
                Content = new HorizontalStackLayout
                {
                    Spacing = 6,
                    Children =
                    {
                        new CheckBox { VerticalOptions = LayoutOptions.Center }
                            .Bind(CheckBox.IsCheckedProperty, nameof(CalendarFilterViewModel.IsSelected),
                                mode: BindingMode.TwoWay),
                        new Label { FontSize = 13, VerticalTextAlignment = TextAlignment.Center }
                            .Bind(Label.TextProperty, nameof(CalendarFilterViewModel.Name)),
                        new Label { FontSize = 11, Opacity = 0.6, VerticalTextAlignment = TextAlignment.Center }
                            .Bind(Label.TextProperty, nameof(CalendarFilterViewModel.Type))
                    }
                }
            };

            chip.Bind(Border.StrokeProperty, nameof(CalendarFilterViewModel.IsSelected),
                converter: new FuncConverter<bool, Color>(s => s ? Colors.DodgerBlue : Colors.LightGray));

            return chip;
        }));

        return layout;
    }

    private static View BuildCalendarModeChips()
    {
        return new HorizontalStackLayout
        {
            Spacing = 12,
            Children =
            {
                new HorizontalStackLayout
                    {
                        Spacing = 4,
                        Children =
                        {
                            new Label
                            {
                                Text = "Available:", FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Colors.Green,
                                VerticalTextAlignment = TextAlignment.Center
                            },
                            new Label { FontSize = 12, Opacity = 0.7, VerticalTextAlignment = TextAlignment.Center }
                                .Bind(Label.TextProperty, nameof(SearchViewModel.AvailableDaysSummary))
                        }
                    }
                    .Bind(IsVisibleProperty, nameof(SearchViewModel.HasAvailableDaysSelected)),
                new HorizontalStackLayout
                    {
                        Spacing = 4,
                        Children =
                        {
                            new Label
                            {
                                Text = "Blocked:", FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Colors.OrangeRed,
                                VerticalTextAlignment = TextAlignment.Center
                            },
                            new Label { FontSize = 12, Opacity = 0.7, VerticalTextAlignment = TextAlignment.Center }
                                .Bind(Label.TextProperty, nameof(SearchViewModel.BlockedDaysSummary))
                        }
                    }
                    .Bind(IsVisibleProperty, nameof(SearchViewModel.HasBlockedDaysSelected))
            }
        };
    }

    private View BuildSourcesSection()
    {
        var header = BuildSectionHeader("Sources",
            nameof(SearchViewModel.SourcesSummary),
            nameof(SearchViewModel.IsSourcesSectionExpanded));

        var content = new VerticalStackLayout
            {
                Padding = new Thickness(16, 0, 16, 12),
                Spacing = 8,
                Children =
                {
                    BuildSourceChips(),
                    BuildSourceFormGroups()
                }
            }
            .Bind(IsVisibleProperty, nameof(SearchViewModel.IsSourcesSectionExpanded));

        return new VerticalStackLayout
        {
            Children = { header, content }
        };
    }

    private static View BuildSourceChips()
    {
        var layout = new FlexLayout
        {
            Wrap = FlexWrap.Wrap,
            JustifyContent = FlexJustify.Start,
            AlignItems = FlexAlignItems.Center
        };

        layout.SetBinding(BindableLayout.ItemsSourceProperty, nameof(SearchViewModel.Sources));

        BindableLayout.SetItemTemplate(layout, new DataTemplate(() =>
        {
            var chip = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 16 },
                Padding = new Thickness(12, 6),
                Margin = new Thickness(0, 0, 8, 8),
                Content = new HorizontalStackLayout
                {
                    Spacing = 6,
                    Children =
                    {
                        new Image { HeightRequest = 20, WidthRequest = 20 }
                            .Bind(Image.SourceProperty, nameof(SourceFilterViewModel.IconFileName)),
                        new CheckBox { VerticalOptions = LayoutOptions.Center }
                            .Bind(CheckBox.IsCheckedProperty, nameof(SourceFilterViewModel.IsSelected),
                                mode: BindingMode.TwoWay),
                        new Label { FontSize = 13, VerticalTextAlignment = TextAlignment.Center }
                            .Bind(Label.TextProperty, nameof(SourceFilterViewModel.Name)),
                        new Label { FontSize = 11, Opacity = 0.5, VerticalTextAlignment = TextAlignment.Center }
                            .Bind(Label.TextProperty, nameof(SourceFilterViewModel.HeaderText))
                    }
                }
            };

            chip.Bind(Border.StrokeProperty, nameof(SourceFilterViewModel.IsSelected),
                converter: new FuncConverter<bool, Color>(s => s ? Colors.DodgerBlue : Colors.LightGray));

            chip.Bind(OpacityProperty, nameof(SourceFilterViewModel.IsInteractable),
                converter: new FuncConverter<bool, double>(interactable => interactable ? 1.0 : 0.5));

            return chip;
        }));

        return layout;
    }

    private static View BuildSourceFormGroups()
    {
        var layout = new VerticalStackLayout { Spacing = 12 };

        layout.SetBinding(BindableLayout.ItemsSourceProperty, nameof(SearchViewModel.FormGroups));

        BindableLayout.SetItemTemplate(layout, new DataTemplate(() =>
        {
            var formGroupLayout = new VerticalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    new Label { FontAttributes = FontAttributes.Bold, FontSize = 13 }
                        .Bind(Label.TextProperty, nameof(FormGroup.Title))
                }
            };

            var elementsLayout = new VerticalStackLayout { Spacing = 6 };
            elementsLayout.SetBinding(BindableLayout.ItemsSourceProperty, nameof(FormGroup.Elements));
            BindableLayout.SetItemTemplate(elementsLayout,
                new DataTemplate(BuildFormElement));

            formGroupLayout.Children.Add(elementsLayout);
            return formGroupLayout;
        }));

        return layout;
    }

    private static View BuildFormElement()
    {
        return new VerticalStackLayout
        {
            Spacing = 4,
            Children =
            {
                new Label { FontSize = 12, Opacity = 0.7 }
                    .Bind(Label.TextProperty, nameof(FormElement.Label)),
                new Entry { FontSize = 14 }
                    .Bind(Entry.TextProperty, nameof(FormElement.TextValue), BindingMode.TwoWay)
            }
        };
    }

    private View BuildFiltersSection()
    {
        var header = BuildSectionHeader("Filters",
            nameof(SearchViewModel.FiltersSummary),
            nameof(SearchViewModel.IsFiltersSectionExpanded));

        var content = new VerticalStackLayout
            {
                Padding = new Thickness(16, 0, 16, 12),
                Spacing = 12,
                Children =
                {
                    new VerticalStackLayout
                    {
                        Spacing = 4,
                        Children =
                        {
                            new Label { Text = "Search phrase", FontSize = 12, Opacity = 0.7 },
                            new Entry { Placeholder = "Search in results...", FontSize = 14 }
                                .Bind(Entry.TextProperty, nameof(SearchViewModel.SearchPhrase), BindingMode.TwoWay)
                        }
                    },
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Star)
                        },
                        ColumnSpacing = 12,
                        Children =
                        {
                            new VerticalStackLayout
                                {
                                    Spacing = 4,
                                    Children =
                                    {
                                        new Label { Text = "Start date", FontSize = 12, Opacity = 0.7 },
                                        new Entry { Placeholder = "yyyy-MM-dd", FontSize = 14, Keyboard = Keyboard.Numeric }
                                            .Bind(Entry.TextProperty, nameof(SearchViewModel.StartDate), BindingMode.TwoWay)
                                    }
                                }
                                .Column(0),
                            new VerticalStackLayout
                                {
                                    Spacing = 4,
                                    Children =
                                    {
                                        new Label { Text = "End date", FontSize = 12, Opacity = 0.7 },
                                        new Entry { Placeholder = "yyyy-MM-dd", FontSize = 14, Keyboard = Keyboard.Numeric }
                                            .Bind(Entry.TextProperty, nameof(SearchViewModel.EndDate), BindingMode.TwoWay)
                                    }
                                }
                                .Column(1)
                        }
                    }
                }
            }
            .Bind(IsVisibleProperty, nameof(SearchViewModel.IsFiltersSectionExpanded));

        return new VerticalStackLayout
        {
            Children = { header, content }
        };
    }

    private View BuildSearchButton()
    {
        return new Button
            {
                Text = "SEARCH",
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(16, 8, 16, 12),
                HeightRequest = 48,
                CornerRadius = 8,
                BackgroundColor = Colors.DodgerBlue,
                TextColor = Colors.White
            }
            .Bind(Button.CommandProperty, nameof(SearchViewModel.SearchCommand));
    }

    private View BuildResultsArea()
    {
        return new Grid
        {
            Children =
            {
                BuildResultsList(),
                BuildNoResultsView(),
                BuildEmptyStateView()
            }
        };
    }

    private View BuildResultsList()
    {
        return new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star)
                },
                Children =
                {
                    BuildSortBar().Row(0),
                    new CollectionView
                        {
                            ItemTemplate = new DataTemplate(BuildResultCard),
                            SelectionMode = SelectionMode.None
                        }
                        .Bind(ItemsView.ItemsSourceProperty, nameof(SearchViewModel.Results))
                        .Margins(8, 0, 8)
                        .Row(1)
                }
            }
            .Bind(IsVisibleProperty, nameof(SearchViewModel.HasResults));
    }

    private static View BuildSortBar()
    {
        return new HorizontalStackLayout
        {
            Padding = new Thickness(16, 8),
            Spacing = 8,
            Children =
            {
                new Label { Text = "Sort by:", FontSize = 13, VerticalTextAlignment = TextAlignment.Center },
                new Picker { FontSize = 13, WidthRequest = 120 }
                    .Bind(Picker.ItemsSourceProperty, nameof(SearchViewModel.SortOptions))
                    .Bind(Picker.SelectedItemProperty, nameof(SearchViewModel.SelectedSortOption), BindingMode.TwoWay)
            }
        };
    }

    private View BuildResultCard()
    {
        var card = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Stroke = Colors.LightGray,
            StrokeThickness = 1,
            Padding = 12,
            Margin = new Thickness(0, 4),
            Content = new VerticalStackLayout
            {
                Spacing = 6,
                Children =
                {
                    BuildResultSourceBadge(),
                    BuildResultDestinationRow(),
                    BuildResultTitle(),
                    BuildResultAdditionalFields(),
                    BuildResultConflicts()
                }
            }
        };

        card.Bind(Border.StrokeProperty, "[HasCalendarOverlaps]",
            converter: new FuncConverter<object, Color>(val =>
                val is true ? Colors.OrangeRed : Colors.LightGray));

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnResultCardTapped;
        card.GestureRecognizers.Add(tapGesture);

        return card;
    }

    private static void OnResultCardTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not View view) return;
        if (view.BindingContext is not Dictionary<string, object> result) return;
        if (result.TryGetValue("Url", out var url) && url is string urlStr && !string.IsNullOrEmpty(urlStr))
        {
            _ = Launcher.OpenAsync(new Uri(urlStr));
        }
    }

    private static View BuildResultSourceBadge()
    {
        return new HorizontalStackLayout
        {
            Spacing = 6,
            Children =
            {
                new Image { HeightRequest = 18, WidthRequest = 18 }
                    .Bind(Image.SourceProperty, "[SourceIconPath]"),
                new Label { FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Colors.DodgerBlue }
                    .Bind(Label.TextProperty, "[SourceName]"),
                new Label { FontSize = 11, Opacity = 0.5 }
                    .Bind(Label.TextProperty, "[SourceLanguage]")
            }
        };
    }

    private static View BuildResultDestinationRow()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            Children =
            {
                new Label { FontSize = 15, FontAttributes = FontAttributes.Bold }
                    .Bind(Label.TextProperty, "[Destination]")
                    .Column(0),
                new HorizontalStackLayout
                    {
                        Spacing = 4,
                        Children =
                        {
                            new Label { FontSize = 12, Opacity = 0.7 }
                                .Bind(Label.TextProperty, "[StartDate]"),
                            new Label { Text = "-", FontSize = 12, Opacity = 0.7 },
                            new Label { FontSize = 12, Opacity = 0.7 }
                                .Bind(Label.TextProperty, "[EndDate]")
                        }
                    }
                    .Column(1)
            }
        };
    }

    private static View BuildResultTitle()
    {
        return new Label { FontSize = 13, Opacity = 0.8, MaxLines = 2, LineBreakMode = LineBreakMode.TailTruncation }
            .Bind(Label.TextProperty, "[Title]");
    }

    private static View BuildResultAdditionalFields()
    {
        return new HorizontalStackLayout
        {
            Spacing = 12,
            Children =
            {
                new Label { FontSize = 12, Opacity = 0.6 }
                    .Bind(Label.TextProperty, "[Type]"),
                new Label { FontSize = 12, FontAttributes = FontAttributes.Bold }
                    .Bind(Label.TextProperty, "[Price]")
            }
        };
    }

    private static View BuildResultConflicts()
    {
        var conflictsLayout = new VerticalStackLayout
        {
            Spacing = 4,
            Padding = new Thickness(8, 6),
            BackgroundColor = Color.FromArgb("#1AFF4444")
        };

        conflictsLayout.SetBinding(BindableLayout.ItemsSourceProperty, "[CalendarOverlaps]");

        BindableLayout.SetItemTemplate(conflictsLayout, new DataTemplate(() =>
            new HorizontalStackLayout
            {
                Spacing = 6,
                Children =
                {
                    new Label { Text = "!", FontSize = 12, TextColor = Colors.OrangeRed, FontAttributes = FontAttributes.Bold },
                    new Label { FontSize = 12, TextColor = Colors.OrangeRed }
                        .Bind(Label.TextProperty, nameof(CalendarOverlapSummary.CalendarName)),
                    new Label { FontSize = 12, Opacity = 0.7 }
                        .Bind(Label.TextProperty, nameof(CalendarOverlapSummary.ConflictSummary))
                }
            }));

        conflictsLayout.Bind(IsVisibleProperty, "[HasCalendarOverlaps]",
            converter: new FuncConverter<object, bool>(val => val is true));

        return conflictsLayout;
    }

    private View BuildNoResultsView()
    {
        return new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label { Text = "No results found", FontSize = 18, HorizontalOptions = LayoutOptions.Center, Opacity = 0.5 },
                    new Label
                    {
                        Text = "Try adjusting your filters", FontSize = 14, HorizontalOptions = LayoutOptions.Center, Opacity = 0.4
                    }
                }
            }
            .Bind(IsVisibleProperty, nameof(SearchViewModel.ShowNoResults));
    }

    private View BuildEmptyStateView()
    {
        return new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label
                    {
                        Text = "Availability Compass", FontSize = 24, HorizontalOptions = LayoutOptions.Center,
                        FontAttributes = FontAttributes.Bold
                    },
                    new Label
                    {
                        Text = "Select sources and search for availability", FontSize = 14, HorizontalOptions = LayoutOptions.Center,
                        Opacity = 0.5
                    }
                }
            }
            .Bind(IsVisibleProperty, nameof(SearchViewModel.HasSearched),
                converter: _inverseBool);
    }
}
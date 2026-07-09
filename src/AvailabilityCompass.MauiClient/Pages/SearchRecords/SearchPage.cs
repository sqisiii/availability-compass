using AvailabilityCompass.Core.Features.SearchRecords;
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.MauiClient.Controls;
using AvailabilityCompass.MauiClient.Messages;
using AvailabilityCompass.MauiClient.Shared.Navigation;
using AvailabilityCompass.MauiClient.Shared.Theme;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace AvailabilityCompass.MauiClient.Pages.SearchRecords;

public class SearchPage : ContentPage
{
    private readonly SearchViewModel _vm;

    public SearchPage(SearchViewModel vm)
    {
        _vm = vm;
        BindingContext = vm;
        Title = "Availability Compass";

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Sources",
            Command = new AsyncRelayCommand(() => NavigationGate.GoToAsync("manage-sources"))
        });
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Calendars",
            Command = new AsyncRelayCommand(() => NavigationGate.GoToAsync("manage-calendars"))
        });

        // The filter area sits in an Auto row and the results in a Star row: nesting the
        // results CollectionView in a page-level ScrollView gave it unbounded height,
        // which disables virtualization and realizes every result card at once.
        Content = SearchTheme.Panel(new Grid
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
        });
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
                SearchTheme.PrimaryLabel(new Label
                    {
                        Text = title, FontAttributes = FontAttributes.Bold, FontSize = 14, VerticalTextAlignment = TextAlignment.Center
                    })
                    .Column(0),
                SearchTheme.MutedLabel(new Label
                    {
                        FontSize = 12, Opacity = 0.6, VerticalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.TailTruncation
                    })
                    .Bind(Label.TextProperty, summaryProperty)
                    .Column(1)
                    .Margins(8),
                SearchTheme.PrimaryLabel(new Label { FontSize = 14, VerticalTextAlignment = TextAlignment.Center })
                    .Bind(Label.TextProperty, expandedProperty,
                        converter: new FuncConverter<bool, string>(expanded => expanded ? "▲" : "▼"))
                    .Column(2)
            }
        };

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (_, _) =>
        {
            WeakReferenceMessenger.Default.Send(new CloseDropdownsMessage());
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
        return SearchTheme.Panel(new VerticalStackLayout
        {
            Spacing = 0,
            Children =
            {
                BuildCalendarsSection(),
                SearchTheme.Divider(new BoxView { HeightRequest = 1 }),
                BuildSourcesSection(),
                SearchTheme.Divider(new BoxView { HeightRequest = 1 }),
                BuildFiltersSection(),
                SearchTheme.Divider(new BoxView { HeightRequest = 1 }),
                BuildSearchButton()
            }
        });
    }

    private View BuildCalendarsSection()
    {
        var header = BuildSectionHeader("Calendars",
            nameof(SearchViewModel.CalendarsSummary),
            nameof(SearchViewModel.IsCalendarsSectionExpanded));

        var content = WrapSectionContent(
            new VerticalStackLayout
            {
                Padding = new Thickness(16, 0, 16, 12),
                Spacing = 8,
                Children =
                {
                    BuildCalendarChips(),
                    BuildCalendarModeChips()
                }
            },
            nameof(SearchViewModel.IsCalendarsSectionExpanded));

        return new VerticalStackLayout
        {
            Children = { header, content }
        };
    }

    /// <summary>
    /// Bounds an expanded section so tall content scrolls internally instead of
    /// pushing the results area off screen (the filter area lives in an Auto row).
    /// </summary>
    private static View WrapSectionContent(View content, string expandedProperty)
    {
        var scroll = new ScrollView
        {
            Content = content,
            MaximumHeightRequest = 360
        };
        scroll.Scrolled += (_, _) => WeakReferenceMessenger.Default.Send(new CloseDropdownsMessage());
        return scroll.Bind(IsVisibleProperty, expandedProperty);
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
                        SearchTheme.PrimaryLabel(new Label { FontSize = 13, VerticalTextAlignment = TextAlignment.Center })
                            .Bind(Label.TextProperty, nameof(CalendarFilterViewModel.Name)),
                        SearchTheme.MutedLabel(new Label { FontSize = 11, Opacity = 0.6, VerticalTextAlignment = TextAlignment.Center })
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
                            SearchTheme.AvailableLabel(new Label
                            {
                                Text = "Available:", FontSize = 12, FontAttributes = FontAttributes.Bold,
                                VerticalTextAlignment = TextAlignment.Center
                            }),
                            SearchTheme.MutedLabel(new Label { FontSize = 12, Opacity = 0.7, VerticalTextAlignment = TextAlignment.Center })
                                .Bind(Label.TextProperty, nameof(SearchViewModel.AvailableDaysSummary))
                        }
                    }
                    .Bind(IsVisibleProperty, nameof(SearchViewModel.HasAvailableDaysSelected)),
                new HorizontalStackLayout
                    {
                        Spacing = 4,
                        Children =
                        {
                            SearchTheme.WarningLabel(new Label
                            {
                                Text = "Blocked:", FontSize = 12, FontAttributes = FontAttributes.Bold,
                                VerticalTextAlignment = TextAlignment.Center
                            }),
                            SearchTheme.MutedLabel(new Label { FontSize = 12, Opacity = 0.7, VerticalTextAlignment = TextAlignment.Center })
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

        var innerContent = new VerticalStackLayout
        {
            Padding = new Thickness(16, 0, 16, 12),
            Spacing = 8,
            Children =
            {
                BuildSourceChips(),
                BuildSourceFormGroups()
            }
        };

        var dismissTap = new TapGestureRecognizer();
        dismissTap.Tapped += (_, _) => WeakReferenceMessenger.Default.Send(new CloseDropdownsMessage());
        innerContent.GestureRecognizers.Add(dismissTap);

        var content = WrapSectionContent(innerContent, nameof(SearchViewModel.IsSourcesSectionExpanded));

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
                Padding = new Thickness(8, 2),
                Margin = new Thickness(0, 0, 6, 6),
                Content = new HorizontalStackLayout
                {
                    Spacing = 2,
                    Children =
                    {
                        new Image { HeightRequest = 16, WidthRequest = 16 }
                            .Bind(Image.SourceProperty, nameof(SourceFilterViewModel.IconFileName)),
                        new CheckBox { VerticalOptions = LayoutOptions.Center }
                            .Bind(CheckBox.IsCheckedProperty, nameof(SourceFilterViewModel.IsSelected),
                                mode: BindingMode.TwoWay),
                        SearchTheme.PrimaryLabel(new Label { FontSize = 12, VerticalTextAlignment = TextAlignment.Center })
                            .Bind(Label.TextProperty, nameof(SourceFilterViewModel.Name))
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
                    SearchTheme.PrimaryLabel(new Label { FontAttributes = FontAttributes.Bold, FontSize = 13 })
                        .Bind(Label.TextProperty, nameof(FormGroup.Title))
                }
            };

            var elementsLayout = new VerticalStackLayout { Spacing = 6 };
            elementsLayout.SetBinding(BindableLayout.ItemsSourceProperty, nameof(FormGroup.Elements));
            BindableLayout.SetItemTemplateSelector(elementsLayout, new FormElementTemplateSelector());

            formGroupLayout.Children.Add(elementsLayout);
            return formGroupLayout;
        }));

        return layout;
    }

    private static View BuildTextBoxFormElement()
    {
        return new VerticalStackLayout
        {
            Spacing = 4,
            Children =
            {
                SearchTheme.MutedLabel(new Label { FontSize = 12, Opacity = 0.7 })
                    .Bind(Label.TextProperty, nameof(FormElement.Label)),
                SearchTheme.SearchEntry(new Entry { FontSize = 14 })
                    .Bind(Entry.TextProperty, nameof(FormElement.TextValue), BindingMode.TwoWay)
            }
        };
    }

    private static View BuildCheckBoxFormElement()
    {
        return new HorizontalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new CheckBox { VerticalOptions = LayoutOptions.Center }
                    .Bind(CheckBox.IsCheckedProperty, nameof(FormElement.TextValue), BindingMode.TwoWay,
                        converter: new FuncConverter<string, bool>(
                            s => string.Equals(s, "true", StringComparison.OrdinalIgnoreCase),
                            b => b ? "true" : "false")),
                SearchTheme.PrimaryLabel(new Label { FontSize = 13, VerticalTextAlignment = TextAlignment.Center })
                    .Bind(Label.TextProperty, nameof(FormElement.Label))
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
                            SearchTheme.MutedLabel(new Label { Text = "Search phrase", FontSize = 12, Opacity = 0.7 }),
                            SearchTheme.SearchEntry(new Entry { Placeholder = "Search in results...", FontSize = 14 })
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
                                        SearchTheme.MutedLabel(new Label { Text = "Start date", FontSize = 12, Opacity = 0.7 }),
                                        SearchTheme.SearchEntry(new Entry { Placeholder = "yyyy-MM-dd", FontSize = 14, Keyboard = Keyboard.Numeric })
                                            .Bind(Entry.TextProperty, nameof(SearchViewModel.StartDate), BindingMode.TwoWay)
                                    }
                                }
                                .Column(0),
                            new VerticalStackLayout
                                {
                                    Spacing = 4,
                                    Children =
                                    {
                                        SearchTheme.MutedLabel(new Label { Text = "End date", FontSize = 12, Opacity = 0.7 }),
                                        SearchTheme.SearchEntry(new Entry { Placeholder = "yyyy-MM-dd", FontSize = 14, Keyboard = Keyboard.Numeric })
                                            .Bind(Entry.TextProperty, nameof(SearchViewModel.EndDate), BindingMode.TwoWay)
                                    }
                                }
                                .Column(1)
                        }
                    }
                }
            };

        return new VerticalStackLayout
        {
            Children = { header, WrapSectionContent(content, nameof(SearchViewModel.IsFiltersSectionExpanded)) }
        };
    }

    private View BuildSearchButton()
    {
        return SearchTheme.PrimaryButton(new Button
            {
                Text = "SEARCH",
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(16, 8, 16, 12),
                HeightRequest = 48,
                CornerRadius = 8
            })
            .Bind(Button.CommandProperty, nameof(SearchViewModel.SearchCommand))
            .Bind(IsEnabledProperty, nameof(SearchViewModel.SourceSelected));
    }

    private View BuildResultsArea()
    {
        return SearchTheme.Panel(new Grid
        {
            Children =
            {
                BuildResultsList(),
                BuildNoResultsView(),
                BuildEmptyStateView(),
                BuildSearchingView()
            }
        });
    }

    private static View BuildSearchingView()
    {
        return new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 12,
                Children =
                {
                    new ActivityIndicator { Color = SearchTheme.Accent, HeightRequest = 40, WidthRequest = 40 }
                        .Bind(ActivityIndicator.IsRunningProperty, nameof(SearchViewModel.ResultsState),
                            converter: new FuncConverter<SearchResultsState, bool>(s => s == SearchResultsState.Searching)),
                    SearchTheme.MutedLabel(new Label
                    {
                        Text = "Searching...", FontSize = 16, HorizontalOptions = LayoutOptions.Center, Opacity = 0.6
                    })
                }
            }
            .Bind(IsVisibleProperty, nameof(SearchViewModel.ResultsState),
                converter: new FuncConverter<SearchResultsState, bool>(s => s == SearchResultsState.Searching));
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
                            SelectionMode = SelectionMode.None,
                            ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem
                        }
                        .Bind(ItemsView.ItemsSourceProperty, nameof(SearchViewModel.Results))
                        .Margins(8, 0, 8)
                        .Row(1)
                }
            }
            .Bind(IsVisibleProperty, nameof(SearchViewModel.ResultsState),
                converter: new FuncConverter<SearchResultsState, bool>(s => s == SearchResultsState.Results));
    }

    private static View BuildSortBar()
    {
        return new HorizontalStackLayout
        {
            Padding = new Thickness(16, 8),
            Spacing = 8,
            Children =
            {
                SearchTheme.PrimaryLabel(new Label { Text = "Sort by:", FontSize = 13, VerticalTextAlignment = TextAlignment.Center }),
                SearchTheme.SearchPicker(new Picker { FontSize = 13, WidthRequest = 120 })
                    .Bind(Picker.ItemsSourceProperty, nameof(SearchViewModel.SortOptions))
                    .Bind(Picker.SelectedItemProperty, nameof(SearchViewModel.SelectedSortOption), BindingMode.TwoWay)
            }
        };
    }

    private View BuildResultCard()
    {
        var card = SearchTheme.Card(new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
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
        });

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
        // TryCreate: a relative/malformed stored Url must not crash the tap handler.
        if (result.TryGetValue("Url", out var url) && url is string urlStr
            && Uri.TryCreate(urlStr, UriKind.Absolute, out var uri))
        {
            _ = Launcher.OpenAsync(uri);
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
                SearchTheme.AccentLabel(new Label { FontSize = 12, FontAttributes = FontAttributes.Bold })
                    .Bind(Label.TextProperty, "[SourceName]"),
                SearchTheme.MutedLabel(new Label { FontSize = 11, Opacity = 0.5 })
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
                SearchTheme.PrimaryLabel(new Label { FontSize = 15, FontAttributes = FontAttributes.Bold })
                    .Bind(Label.TextProperty, "[Destination]")
                    .Column(0),
                new HorizontalStackLayout
                    {
                        Spacing = 4,
                        Children =
                        {
                            SearchTheme.MutedLabel(new Label { FontSize = 12, Opacity = 0.7 })
                                .Bind(Label.TextProperty, "[StartDate]"),
                            SearchTheme.MutedLabel(new Label { Text = "-", FontSize = 12, Opacity = 0.7 }),
                            SearchTheme.MutedLabel(new Label { FontSize = 12, Opacity = 0.7 })
                                .Bind(Label.TextProperty, "[EndDate]")
                        }
                    }
                    .Column(1)
            }
        };
    }

    private static View BuildResultTitle()
    {
        return SearchTheme.PrimaryLabel(new Label { FontSize = 13, Opacity = 0.8, MaxLines = 2, LineBreakMode = LineBreakMode.TailTruncation })
            .Bind(Label.TextProperty, "[Title]");
    }

    private static View BuildResultAdditionalFields()
    {
        return new HorizontalStackLayout
        {
            Spacing = 12,
            Children =
            {
                SearchTheme.MutedLabel(new Label { FontSize = 12, Opacity = 0.6 })
                    .Bind(Label.TextProperty, "[Type]"),
                SearchTheme.PrimaryLabel(new Label { FontSize = 12, FontAttributes = FontAttributes.Bold })
                    .Bind(Label.TextProperty, "[Price]")
            }
        };
    }

    private static View BuildResultConflicts()
    {
        var conflictsLayout = SearchTheme.ConflictPanel(new VerticalStackLayout
        {
            Spacing = 4,
            Padding = new Thickness(8, 6)
        });

        conflictsLayout.SetBinding(BindableLayout.ItemsSourceProperty, "[CalendarOverlaps]");

        BindableLayout.SetItemTemplate(conflictsLayout, new DataTemplate(() =>
            new VerticalStackLayout
            {
                Spacing = 2,
                Children =
                {
                    new HorizontalStackLayout
                    {
                        Spacing = 6,
                        Children =
                        {
                            SearchTheme.WarningLabel(new Label { Text = "!", FontSize = 12, FontAttributes = FontAttributes.Bold }),
                            SearchTheme.WarningLabel(new Label { FontSize = 12, FontAttributes = FontAttributes.Bold })
                                .Bind(Label.TextProperty, nameof(CalendarOverlapSummary.CalendarName))
                        }
                    },
                    SearchTheme.WarningLabel(new Label
                        {
                            FontSize = 11,
                            Opacity = 0.85,
                            LineBreakMode = LineBreakMode.WordWrap
                        })
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
                    SearchTheme.MutedLabel(new Label { Text = "No results found", FontSize = 18, HorizontalOptions = LayoutOptions.Center, Opacity = 0.5 }),
                    SearchTheme.MutedLabel(new Label
                    {
                        Text = "Try adjusting your filters", FontSize = 14, HorizontalOptions = LayoutOptions.Center, Opacity = 0.4
                    })
                }
            }
            .Bind(IsVisibleProperty, nameof(SearchViewModel.ResultsState),
                converter: new FuncConverter<SearchResultsState, bool>(s => s == SearchResultsState.NoResults));
    }

    private View BuildEmptyStateView()
    {
        return new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    SearchTheme.PrimaryLabel(new Label
                    {
                        Text = "Availability Compass", FontSize = 24, HorizontalOptions = LayoutOptions.Center,
                        FontAttributes = FontAttributes.Bold
                    }),
                    SearchTheme.MutedLabel(new Label
                    {
                        Text = "Select sources and search for availability", FontSize = 14, HorizontalOptions = LayoutOptions.Center,
                        Opacity = 0.5
                    })
                }
            }
            .Bind(IsVisibleProperty, nameof(SearchViewModel.ResultsState),
                converter: new FuncConverter<SearchResultsState, bool>(s => s == SearchResultsState.EmptyState));
    }

    private class FormElementTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate _checkBoxTemplate = new(BuildCheckBoxFormElement);
        private readonly DataTemplate _multiSelectTemplate = new(() => new MultiSelectDropdown());
        private readonly DataTemplate _textBoxTemplate = new(BuildTextBoxFormElement);

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is not FormElement element) return _textBoxTemplate;
            return element.Type switch
            {
                FormElementType.CheckBox => _checkBoxTemplate,
                FormElementType.MultiSelect => _multiSelectTemplate,
                _ => _textBoxTemplate
            };
        }
    }
}

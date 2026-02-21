using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.MauiClient.Controls.Calendar;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;

namespace AvailabilityCompass.MauiClient.Pages.ManageCalendars;

public class ManageCalendarsPage : ContentPage
{
    private static readonly Color SingleDateColor = Color.FromArgb("#8B0000");
    private static readonly Color RecurringDateColor = Color.FromArgb("#E9967A");

    private readonly ManageCalendarsViewModel _vm;
    private CalendarView? _calendarView;

    public ManageCalendarsPage(ManageCalendarsViewModel vm)
    {
        _vm = vm;
        BindingContext = vm;
        Title = "Manage Calendars";

        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);

        Content = new Grid
        {
            Children =
            {
                BuildMainContent(),
                BuildEditorOverlay(),
                BuildDeleteConfirmationOverlay()
            }
        };

        vm.ReservedDates.CollectionChanged += (_, _) => UpdateCalendarDecorations();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _ = _vm.LoadDataAsync(CancellationToken.None);
    }

    private void UpdateCalendarDecorations()
    {
        if (_calendarView is null) return;

        var decorations = _vm.ReservedDates.Select(rd => new CalendarDateDecoration(
                DateOnly.FromDateTime(rd.Date),
                rd.Category == CategorizedDateCategory.SingleDate ? SingleDateColor : RecurringDateColor,
                rd.Tooltip))
            .ToList();

        _calendarView.Decorations = decorations;
    }

    #region Delete Confirmation Overlay

    private static View BuildDeleteConfirmationOverlay()
    {
        return new Grid
            {
                BackgroundColor = Color.FromArgb("#80000000"),
                Children =
                {
                    new Border
                    {
                        StrokeShape = new RoundRectangle { CornerRadius = 16 },
                        Stroke = Colors.Red,
                        BackgroundColor = Colors.White,
                        Margin = new Thickness(40),
                        Padding = 24,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        MaximumWidthRequest = 350,
                        Content = new VerticalStackLayout
                        {
                            Spacing = 16,
                            Children =
                            {
                                new Label
                                {
                                    Text = "Delete Calendar?", FontAttributes = FontAttributes.Bold, FontSize = 18, TextColor = Colors.Red
                                },
                                new Label { FontSize = 14 }
                                    .Bind(Label.TextProperty, nameof(ManageCalendarsViewModel.DeleteCalendarName),
                                        converter: new FuncConverter<string, string>(name =>
                                            $"Are you sure you want to delete \"{name}\" and all its date entries?")),
                                new HorizontalStackLayout
                                {
                                    Spacing = 12,
                                    HorizontalOptions = LayoutOptions.End,
                                    Children =
                                    {
                                        new Button { Text = "Cancel", BackgroundColor = Colors.Gray }
                                            .Bind(Button.CommandProperty, nameof(ManageCalendarsViewModel.CancelDeleteCalendarCommand)),
                                        new Button { Text = "Delete", BackgroundColor = Colors.Red, TextColor = Colors.White }
                                            .Bind(Button.CommandProperty, nameof(ManageCalendarsViewModel.ConfirmDeleteCalendarCommand))
                                    }
                                }
                            }
                        }
                    }
                }
            }
            .Bind(IsVisibleProperty, nameof(ManageCalendarsViewModel.IsDeleteConfirmationOpen));
    }

    #endregion

    #region Main Content

    private View BuildMainContent()
    {
        return new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 0,
                Children =
                {
                    BuildCalendarSelector(),
                    BuildAddCalendarForm(),
                    BuildEditCalendarForm(),
                    BuildCalendarContent(),
                    BuildDateEntriesList()
                }
            }
        };
    }

    private View BuildCalendarSelector()
    {
        var chipLayout = new HorizontalStackLayout
        {
            Spacing = 8,
            Padding = new Thickness(16, 12)
        };

        chipLayout.SetBinding(BindableLayout.ItemsSourceProperty, nameof(ManageCalendarsViewModel.Calendars));

        BindableLayout.SetItemTemplate(chipLayout, new DataTemplate(() =>
        {
            var chip = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 20 },
                Padding = new Thickness(16, 8),
                Content = new VerticalStackLayout
                {
                    Spacing = 2,
                    Children =
                    {
                        new Label { FontSize = 13, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center }
                            .Bind(Label.TextProperty, nameof(CalendarViewModel.Name)),
                        new Label { FontSize = 10, Opacity = 0.6, HorizontalTextAlignment = TextAlignment.Center }
                            .Bind(Label.TextProperty, nameof(CalendarViewModel.Type))
                    }
                }
            };

            chip.Bind(Border.StrokeProperty, nameof(CalendarViewModel.IsSelected),
                converter: new FuncConverter<bool, Color>(s => s ? Colors.DodgerBlue : Colors.LightGray));
            chip.Bind(BackgroundColorProperty, nameof(CalendarViewModel.IsSelected),
                converter: new FuncConverter<bool, Color>(s => s ? Color.FromArgb("#E8F0FE") : Colors.Transparent));

            var tap = new TapGestureRecognizer();
            tap.Tapped += (sender, _) =>
            {
                if (sender is not View v || v.BindingContext is not CalendarViewModel cal) return;
                foreach (var c in _vm.Calendars)
                    c.IsSelected = false;
                cal.IsSelected = true;
            };
            chip.GestureRecognizers.Add(tap);

            return chip;
        }));

        var addButton = new Button
        {
            Text = "+",
            FontSize = 18,
            WidthRequest = 44,
            HeightRequest = 44,
            CornerRadius = 22,
            Padding = 0
        };
        addButton.SetBinding(Button.CommandProperty,
            new Binding(nameof(ManageCalendarsViewModel.ExpandAddCalendarCommand)));

        var container = new HorizontalStackLayout
        {
            Spacing = 8,
            Children = { chipLayout, addButton }
        };

        return new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 8),
            Content = container
        };
    }

    private static View BuildAddCalendarForm()
    {
        return new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 8 },
                Stroke = Colors.LightGray,
                Margin = new Thickness(16, 0, 16, 8),
                Padding = 12,
                Content = new VerticalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                        new Label { Text = "Add Calendar", FontAttributes = FontAttributes.Bold, FontSize = 15 },
                        new Entry { Placeholder = "Calendar name (max 16 chars)", MaxLength = 16, FontSize = 14 }
                            .Bind(Entry.TextProperty, nameof(ManageCalendarsViewModel.NewCalendarName), BindingMode.TwoWay),
                        new HorizontalStackLayout
                        {
                            Spacing = 8,
                            Children =
                            {
                                new CheckBox()
                                    .Bind(CheckBox.IsCheckedProperty, nameof(ManageCalendarsViewModel.NewCalendarIsOnly),
                                        BindingMode.TwoWay),
                                new Label { Text = "Available days only", FontSize = 13, VerticalTextAlignment = TextAlignment.Center }
                            }
                        },
                        new HorizontalStackLayout
                        {
                            Spacing = 8,
                            Children =
                            {
                                new Button { Text = "Save" }
                                    .Bind(Button.CommandProperty, nameof(ManageCalendarsViewModel.AddCalendarInlineCommand)),
                                new Button { Text = "Cancel", BackgroundColor = Colors.Gray }
                                    .Bind(Button.CommandProperty, nameof(ManageCalendarsViewModel.CancelAddCalendarCommand))
                            }
                        }
                    }
                }
            }
            .Bind(IsVisibleProperty, nameof(ManageCalendarsViewModel.IsAddCalendarExpanded));
    }

    private static View BuildEditCalendarForm()
    {
        return new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 8 },
                Stroke = Colors.DodgerBlue,
                Margin = new Thickness(16, 0, 16, 8),
                Padding = 12,
                Content = new VerticalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                        new Label { Text = "Edit Calendar", FontAttributes = FontAttributes.Bold, FontSize = 15 },
                        new Entry { Placeholder = "Calendar name", MaxLength = 16, FontSize = 14 }
                            .Bind(Entry.TextProperty, nameof(ManageCalendarsViewModel.EditCalendarName), BindingMode.TwoWay),
                        new HorizontalStackLayout
                        {
                            Spacing = 8,
                            Children =
                            {
                                new CheckBox()
                                    .Bind(CheckBox.IsCheckedProperty, nameof(ManageCalendarsViewModel.EditCalendarIsOnly),
                                        BindingMode.TwoWay),
                                new Label { Text = "Available days only", FontSize = 13, VerticalTextAlignment = TextAlignment.Center }
                            }
                        },
                        new HorizontalStackLayout
                        {
                            Spacing = 8,
                            Children =
                            {
                                new Button { Text = "Save" }
                                    .Bind(Button.CommandProperty, nameof(ManageCalendarsViewModel.SaveCalendarEditCommand)),
                                new Button { Text = "Cancel", BackgroundColor = Colors.Gray }
                                    .Bind(Button.CommandProperty, nameof(ManageCalendarsViewModel.CancelCalendarEditCommand)),
                                new Button { Text = "Delete", BackgroundColor = Colors.Red, TextColor = Colors.White }
                                    .Bind(Button.CommandProperty, nameof(ManageCalendarsViewModel.DeleteCalendarCommand))
                                    .Bind(Button.CommandParameterProperty, nameof(ManageCalendarsViewModel.SelectedCalendarId))
                            }
                        }
                    }
                }
            }
            .Bind(IsVisibleProperty, nameof(ManageCalendarsViewModel.IsEditCalendarExpanded));
    }

    private View BuildCalendarContent()
    {
        _calendarView = new CalendarView
        {
            Margin = new Thickness(16, 8)
        };
        _calendarView.SetBinding(CalendarView.SelectedDatesProperty,
            nameof(ManageCalendarsViewModel.SelectedDates));
        _calendarView.SetBinding(CalendarView.HasSelectedDatesProperty,
            new Binding(nameof(ManageCalendarsViewModel.HasSelectedDates), BindingMode.TwoWay));
        _calendarView.SetBinding(CalendarView.DateClickedCommandProperty,
            nameof(ManageCalendarsViewModel.DateClickedCommand));

        var legend = new HorizontalStackLayout
        {
            Spacing = 16,
            Margin = new Thickness(16, 4, 16, 0),
            Children =
            {
                new HorizontalStackLayout
                {
                    Spacing = 4,
                    Children =
                    {
                        new BoxView { Color = SingleDateColor, WidthRequest = 12, HeightRequest = 12, CornerRadius = 2 },
                        new Label { Text = "Single dates", FontSize = 11, Opacity = 0.7, VerticalTextAlignment = TextAlignment.Center }
                    }
                },
                new HorizontalStackLayout
                {
                    Spacing = 4,
                    Children =
                    {
                        new BoxView { Color = RecurringDateColor, WidthRequest = 12, HeightRequest = 12, CornerRadius = 2 },
                        new Label { Text = "Recurring dates", FontSize = 11, Opacity = 0.7, VerticalTextAlignment = TextAlignment.Center }
                    }
                }
            }
        };

        var editButton = new Button
            {
                Text = "Edit Calendar",
                FontSize = 13,
                Margin = new Thickness(16, 4, 16, 0)
            }
            .Bind(Button.CommandProperty, nameof(ManageCalendarsViewModel.UpdateCalendarCommand))
            .Bind(Button.CommandParameterProperty, nameof(ManageCalendarsViewModel.SelectedCalendarId));

        var addDatesButton = new Button
            {
                Text = "Add Selected Dates",
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(16, 8, 16, 0),
                BackgroundColor = Colors.DodgerBlue,
                TextColor = Colors.White
            }
            .Bind(Button.CommandProperty, nameof(ManageCalendarsViewModel.AddSelectedDatesCommand))
            .Bind(Button.CommandParameterProperty, nameof(ManageCalendarsViewModel.SelectedDates))
            .Bind(IsVisibleProperty, nameof(ManageCalendarsViewModel.HasSelectedDates));

        return new VerticalStackLayout
            {
                Children = { _calendarView, legend, editButton, addDatesButton }
            }
            .Bind(IsVisibleProperty, nameof(ManageCalendarsViewModel.IsCalendarSelected));
    }

    private View BuildDateEntriesList()
    {
        var header = new Label
        {
            Text = "Date Entries",
            FontAttributes = FontAttributes.Bold,
            FontSize = 15,
            Margin = new Thickness(16, 16, 16, 8)
        };

        var listLayout = new VerticalStackLayout
        {
            Spacing = 6,
            Padding = new Thickness(16, 0)
        };

        listLayout.SetBinding(BindableLayout.ItemsSourceProperty,
            nameof(ManageCalendarsViewModel.DateEntries));

        BindableLayout.SetItemTemplate(listLayout, new DataTemplate(() =>
        {
            var card = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 8 },
                Stroke = Colors.LightGray,
                Padding = 10,
                Content = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto)
                    },
                    Children =
                    {
                        new VerticalStackLayout
                            {
                                Spacing = 2,
                                Children =
                                {
                                    new Label { FontSize = 13, FontAttributes = FontAttributes.Bold }
                                        .Bind(Label.TextProperty, nameof(DateEntryViewModel.Description)),
                                    new Label { FontSize = 12, Opacity = 0.7 }
                                        .Bind(Label.TextProperty, nameof(DateEntryViewModel.DateRangeDisplay)),
                                    new HorizontalStackLayout
                                        {
                                            Spacing = 8,
                                            Children =
                                            {
                                                new Label { FontSize = 11, TextColor = RecurringDateColor }
                                                    .Bind(Label.TextProperty, nameof(DateEntryViewModel.IsRecurring),
                                                        converter: new FuncConverter<bool, string>(r => r ? "Recurring" : "")),
                                                new Label { FontSize = 11, Opacity = 0.5 }
                                                    .Bind(Label.TextProperty, nameof(DateEntryViewModel.NextOccurrenceDisplay))
                                            }
                                        }
                                        .Bind(IsVisibleProperty, nameof(DateEntryViewModel.IsRecurring))
                                }
                            }
                            .Column(0),
                        new Label
                            {
                                Text = ">",
                                FontSize = 18,
                                Opacity = 0.4,
                                VerticalTextAlignment = TextAlignment.Center
                            }
                            .Column(1)
                    }
                }
            };

            var tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandProperty,
                new Binding(nameof(ManageCalendarsViewModel.EditEntryCommand), source: _vm));
            tap.SetBinding(TapGestureRecognizer.CommandParameterProperty, new Binding("."));
            card.GestureRecognizers.Add(tap);

            return card;
        }));

        return new VerticalStackLayout
            {
                Children = { header, listLayout }
            }
            .Bind(IsVisibleProperty, nameof(ManageCalendarsViewModel.IsCalendarSelected));
    }

    #endregion

    #region Editor Overlay

    private View BuildEditorOverlay()
    {
        return new Grid
            {
                BackgroundColor = Color.FromArgb("#80000000"),
                Children =
                {
                    new Border
                    {
                        StrokeShape = new RoundRectangle { CornerRadius = 16 },
                        Stroke = Colors.LightGray,
                        BackgroundColor = Colors.White,
                        Margin = new Thickness(24),
                        Padding = 20,
                        VerticalOptions = LayoutOptions.Center,
                        MaximumWidthRequest = 400,
                        Content = new VerticalStackLayout
                        {
                            Spacing = 12,
                            Children =
                            {
                                new Label { FontAttributes = FontAttributes.Bold, FontSize = 18 }
                                    .Bind(Label.TextProperty, nameof(ManageCalendarsViewModel.EditorTitle)),
                                BuildDetectedSelectionsView(),
                                new Label { Text = "Description", FontSize = 12, Opacity = 0.7 },
                                new Entry { Placeholder = "Enter description...", FontSize = 14 }
                                    .Bind(Entry.TextProperty, nameof(ManageCalendarsViewModel.EditorDescription), BindingMode.TwoWay),
                                new HorizontalStackLayout
                                {
                                    Spacing = 8,
                                    Children =
                                    {
                                        new CheckBox()
                                            .Bind(CheckBox.IsCheckedProperty, nameof(ManageCalendarsViewModel.EditorIsRecurring),
                                                BindingMode.TwoWay),
                                        new Label { Text = "Recurring", FontSize = 13, VerticalTextAlignment = TextAlignment.Center }
                                    }
                                },
                                BuildRecurringFields(),
                                new Label { FontSize = 12, TextColor = Colors.Red }
                                    .Bind(Label.TextProperty, nameof(ManageCalendarsViewModel.FrequencyError)),
                                new Label { FontSize = 12, TextColor = Colors.Red }
                                    .Bind(Label.TextProperty, nameof(ManageCalendarsViewModel.RepetitionsError)),
                                BuildEditorButtons()
                            }
                        }
                    }
                }
            }
            .Bind(IsVisibleProperty, nameof(ManageCalendarsViewModel.IsEditorOpen));
    }

    private static View BuildDetectedSelectionsView()
    {
        var layout = new VerticalStackLayout { Spacing = 4 };

        layout.SetBinding(BindableLayout.ItemsSourceProperty,
            nameof(ManageCalendarsViewModel.EditorDetectedSelections));

        BindableLayout.SetItemTemplate(layout, new DataTemplate(() =>
            new Label { FontSize = 12, Opacity = 0.7 }
                .Bind(Label.TextProperty, nameof(DetectedSelection.DisplayText))));

        return layout;
    }

    private static View BuildRecurringFields()
    {
        return new Grid
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
                                new Label { Text = "Frequency (days)", FontSize = 12, Opacity = 0.7 },
                                new Entry { Keyboard = Keyboard.Numeric, FontSize = 14 }
                                    .Bind(Entry.TextProperty, nameof(ManageCalendarsViewModel.EditorFrequency), BindingMode.TwoWay)
                            }
                        }
                        .Column(0),
                    new VerticalStackLayout
                        {
                            Spacing = 4,
                            Children =
                            {
                                new Label { Text = "Repetitions", FontSize = 12, Opacity = 0.7 },
                                new Entry { Keyboard = Keyboard.Numeric, FontSize = 14 }
                                    .Bind(Entry.TextProperty, nameof(ManageCalendarsViewModel.EditorRepetitions), BindingMode.TwoWay)
                            }
                        }
                        .Column(1)
                }
            }
            .Bind(IsVisibleProperty, nameof(ManageCalendarsViewModel.EditorIsRecurring));
    }

    private View BuildEditorButtons()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 8,
            Children =
            {
                new Button
                    {
                        Text = "Delete",
                        BackgroundColor = Colors.Red,
                        TextColor = Colors.White
                    }
                    .Bind(Button.CommandProperty, nameof(ManageCalendarsViewModel.DeleteEntryCommand))
                    .Bind(IsVisibleProperty, nameof(ManageCalendarsViewModel.IsEditMode))
                    .Column(0),
                new BoxView { Color = Colors.Transparent }.Column(1),
                new Button
                    {
                        Text = "Cancel",
                        BackgroundColor = Colors.Gray
                    }
                    .Bind(Button.CommandProperty, nameof(ManageCalendarsViewModel.CancelEditCommand))
                    .Column(2),
                new Button
                    {
                        Text = "Save",
                        BackgroundColor = Colors.DodgerBlue,
                        TextColor = Colors.White
                    }
                    .Bind(Button.CommandProperty, nameof(ManageCalendarsViewModel.SaveEntryCommand))
                    .Column(3)
            }
        };
    }

    #endregion
}
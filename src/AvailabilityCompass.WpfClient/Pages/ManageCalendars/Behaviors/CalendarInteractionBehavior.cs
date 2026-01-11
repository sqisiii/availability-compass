using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.WpfClient.Shared;
using Microsoft.Xaml.Behaviors;

namespace AvailabilityCompass.WpfClient.Pages.ManageCalendars.Behaviors;

/// <summary>
/// A behavior that provides interactive calendar functionality including color coding,
/// date click handling, and selection support.
/// </summary>
public class CalendarInteractionBehavior : Behavior<Calendar>
{
    public static readonly DependencyProperty ColorCodedDatesProperty =
        DependencyProperty.Register(nameof(ColorCodedDates), typeof(ObservableCollection<CategorizedDate>),
            typeof(CalendarInteractionBehavior),
            new PropertyMetadata(new ObservableCollection<CategorizedDate>(), OnDatesChanged));

    public static readonly DependencyProperty DateClickedCommandProperty =
        DependencyProperty.Register(nameof(DateClickedCommand), typeof(ICommand), typeof(CalendarInteractionBehavior));

    // Local category-to-brush mapping
    private readonly Dictionary<CategorizedDateCategory, Brush> _categoryBrushes = new()
    {
        { CategorizedDateCategory.SingleDate, Brushes.DarkRed },
        { CategorizedDateCategory.RecurringDate, Brushes.DarkSalmon },
        { CategorizedDateCategory.Inverted, Brushes.Purple }
    };

    public ObservableCollection<CategorizedDate> ColorCodedDates
    {
        get => (ObservableCollection<CategorizedDate>)GetValue(ColorCodedDatesProperty);
        set => SetValue(ColorCodedDatesProperty, value);
    }

    public ICommand? DateClickedCommand
    {
        get => (ICommand?)GetValue(DateClickedCommandProperty);
        set => SetValue(DateClickedCommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += AssociatedObject_Loaded;
        AssociatedObject.DisplayDateChanged += AssociatedObject_DisplayDateChanged;
    }

    private void AssociatedObject_DisplayDateChanged(object? sender, CalendarDateChangedEventArgs e)
    {
        ApplyColorsAndHookButtons();
    }

    private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
    {
        ApplyColorsAndHookButtons();
    }

    private static void OnDatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CalendarInteractionBehavior behavior)
        {
            return;
        }

        if (e.OldValue is ObservableCollection<CategorizedDate> oldCollection)
            oldCollection.CollectionChanged -= behavior.CategorizedDate_CollectionChanged;

        if (e.NewValue is ObservableCollection<CategorizedDate> newCollection)
            newCollection.CollectionChanged += behavior.CategorizedDate_CollectionChanged;

        behavior.ApplyColorsAndHookButtons();
    }

    private void CategorizedDate_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ApplyColorsAndHookButtons();
    }

    private void ApplyColorsAndHookButtons()
    {
        if (AssociatedObject.Template.FindName("PART_CalendarItem", AssociatedObject) is not CalendarItem calendarItem)
        {
            return;
        }

        calendarItem.ApplyTemplate();
        var buttons = calendarItem.FindVisualChildren<CalendarDayButton>();

        foreach (var button in buttons)
        {
            // Unhook to avoid duplicate handlers
            button.PreviewMouseLeftButtonUp -= OnDayButtonClicked;
            button.PreviewMouseLeftButtonUp += OnDayButtonClicked;

            if (button.DataContext is not DateTime date)
            {
                continue;
            }

            var match = ColorCodedDates.FirstOrDefault(x => x.Date.Date == date.Date);
            if (match != null)
            {
                button.Background = _categoryBrushes.TryGetValue(match.Category, out var brush) ? brush : Brushes.Gray;
                button.Foreground = Brushes.White;
                ToolTipService.SetToolTip(button, match.Tooltip);
            }
            else
            {
                button.ClearValue(Control.BackgroundProperty);
                button.ClearValue(Control.ForegroundProperty);
                ToolTipService.SetToolTip(button, null);
            }
        }
    }

    private void OnDayButtonClicked(object sender, MouseButtonEventArgs e)
    {
        if (sender is not CalendarDayButton button)
        {
            return;
        }

        if (button.DataContext is not DateTime date)
        {
            return;
        }

        // Only fire command if clicking an existing entry (colored date)
        // For non-colored dates, let Calendar handle selection normally
        var match = ColorCodedDates.FirstOrDefault(x => x.Date.Date == date.Date);
        if (match == null || DateClickedCommand?.CanExecute(date) != true)
        {
            return;
        }

        DateClickedCommand.Execute(date);
        e.Handled = true; // Prevent selection change
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Loaded -= AssociatedObject_Loaded;
        AssociatedObject.DisplayDateChanged -= AssociatedObject_DisplayDateChanged;

        // Clean up button event handlers
        if (AssociatedObject.Template.FindName("PART_CalendarItem", AssociatedObject) is CalendarItem calendarItem)
        {
            calendarItem.ApplyTemplate();
            var buttons = calendarItem.FindVisualChildren<CalendarDayButton>();
            foreach (var button in buttons)
            {
                button.PreviewMouseLeftButtonUp -= OnDayButtonClicked;
            }
        }
    }
}
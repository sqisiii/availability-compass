using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.WpfClient.Shared;
using Microsoft.Xaml.Behaviors;

namespace AvailabilityCompass.WpfClient.Pages.ManageCalendars.Behaviors;

/// <summary>
/// A behavior that applies color coding and hints to dates in a WPF Calendar control based on their categories.
/// </summary>
public class CalendarColorBehavior : Behavior<Calendar>
{
    public static readonly DependencyProperty ColorCodedDatesProperty =
        DependencyProperty.Register(nameof(ColorCodedDates), typeof(ObservableCollection<CategorizedDate>), typeof(CalendarColorBehavior),
            new PropertyMetadata(new ObservableCollection<CategorizedDate>(), OnDatesChanged));

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

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += AssociatedObject_Loaded;
        AssociatedObject.DisplayDateChanged += AssociatedObject_DisplayDateChanged;
    }

    private void AssociatedObject_DisplayDateChanged(object? sender, CalendarDateChangedEventArgs e)
    {
        ApplyColors();
    }

    private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
    {
        ApplyColors();
    }

    private static void OnDatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CalendarColorBehavior behavior)
        {
            return;
        }

        if (e.OldValue is ObservableCollection<CategorizedDate> oldCollection)
            oldCollection.CollectionChanged -= behavior.CategorizedDate_CollectionChanged;

        if (e.NewValue is ObservableCollection<CategorizedDate> newCollection)
            newCollection.CollectionChanged += behavior.CategorizedDate_CollectionChanged;

        behavior.ApplyColors();
    }

    private void CategorizedDate_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ApplyColors();
    }

    private void ApplyColors()
    {
        if (AssociatedObject.Template.FindName("PART_CalendarItem", AssociatedObject) is not CalendarItem calendarItem)
        {
            return;
        }

        calendarItem.ApplyTemplate();
        var buttons = calendarItem.FindVisualChildren<CalendarDayButton>();

        foreach (var button in buttons)
        {
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

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Loaded -= AssociatedObject_Loaded;
        AssociatedObject.DisplayDateChanged -= AssociatedObject_DisplayDateChanged;
    }
}
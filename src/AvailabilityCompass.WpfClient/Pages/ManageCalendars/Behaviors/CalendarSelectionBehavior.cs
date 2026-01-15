using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace AvailabilityCompass.WpfClient.Pages.ManageCalendars.Behaviors;

/// <summary>
/// A behavior that tracks calendar selection state and stores selected dates
/// </summary>
public class CalendarSelectionBehavior : Behavior<Calendar>
{
    public static readonly DependencyProperty HasSelectedDatesProperty =
        DependencyProperty.Register(
            nameof(HasSelectedDates),
            typeof(bool),
            typeof(CalendarSelectionBehavior),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty SelectedDatesProperty =
        DependencyProperty.Register(
            nameof(SelectedDates),
            typeof(IList),
            typeof(CalendarSelectionBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool HasSelectedDates
    {
        get => (bool)GetValue(HasSelectedDatesProperty);
        set => SetValue(HasSelectedDatesProperty, value);
    }

    public IList? SelectedDates
    {
        get => (IList?)GetValue(SelectedDatesProperty);
        set => SetValue(SelectedDatesProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SelectedDatesChanged += OnSelectedDatesChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.SelectedDatesChanged -= OnSelectedDatesChanged;
    }

    private void OnSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        var count = AssociatedObject.SelectedDates.Count;
        HasSelectedDates = count > 0;

        // Store a copy of selected dates so they're available even after focus changes
        if (count > 0)
        {
            var datesCopy = new ObservableCollection<DateTime>();
            foreach (DateTime date in AssociatedObject.SelectedDates)
            {
                datesCopy.Add(date);
            }

            SelectedDates = datesCopy;
        }
        else
        {
            SelectedDates = null;
        }

        // Release mouse capture to prevent double-click issue on external buttons
        if (Mouse.Captured is Calendar || Mouse.Captured is CalendarItem)
        {
            Mouse.Capture(null);
        }
    }
}
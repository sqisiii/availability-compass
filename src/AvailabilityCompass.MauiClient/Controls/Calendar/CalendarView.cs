using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;

namespace AvailabilityCompass.MauiClient.Controls.Calendar;

/// <summary>
/// A cross-platform calendar control with date decoration and multi-select support.
/// Renders as a pure C# Grid layout - no platform-specific handlers needed.
/// </summary>
public class CalendarView : ContentView
{
    private static readonly Color SelectedColor = Colors.DodgerBlue;
    private static readonly Color TodayColor = Color.FromArgb("#E8E8FF");

    public static readonly BindableProperty DecorationsProperty =
        BindableProperty.Create(nameof(Decorations), typeof(IEnumerable), typeof(CalendarView),
            propertyChanged: OnDecorationsChanged);

    public static readonly BindableProperty SelectedDatesProperty =
        BindableProperty.Create(nameof(SelectedDates), typeof(IList), typeof(CalendarView));

    public static readonly BindableProperty HasSelectedDatesProperty =
        BindableProperty.Create(nameof(HasSelectedDates), typeof(bool), typeof(CalendarView), false);

    public static readonly BindableProperty DateClickedCommandProperty =
        BindableProperty.Create(nameof(DateClickedCommand), typeof(ICommand), typeof(CalendarView));

    public static readonly BindableProperty FirstDayOfWeekProperty =
        BindableProperty.Create(nameof(FirstDayOfWeek), typeof(DayOfWeek), typeof(CalendarView),
            DayOfWeek.Monday, propertyChanged: (b, _, _) => ((CalendarView)b).Render());

    private readonly Grid _dayGrid;
    private readonly Label _monthLabel;

    private readonly HashSet<DateOnly> _selectedDates = [];

    private DateOnly _displayMonth;

    public CalendarView()
    {
        _displayMonth = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1);

        _monthLabel = new Label
        {
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        var prevButton = new Button { Text = "<", WidthRequest = 44, HeightRequest = 36, Padding = 0 };
        prevButton.Clicked += (_, _) => NavigateMonth(-1);

        var nextButton = new Button { Text = ">", WidthRequest = 44, HeightRequest = 36, Padding = 0 };
        nextButton.Clicked += (_, _) => NavigateMonth(1);

        var navBar = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            Children =
            {
                prevButton,
                _monthLabel,
                nextButton
            }
        };
        Grid.SetColumn(_monthLabel, 1);
        Grid.SetColumn(nextButton, 2);

        _dayGrid = new Grid
        {
            RowSpacing = 2,
            ColumnSpacing = 2
        };

        // 7 columns for days of week
        for (var i = 0; i < 7; i++)
            _dayGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        var root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            },
            RowSpacing = 8,
            Children = { navBar, _dayGrid }
        };
        Grid.SetRow(_dayGrid, 1);

        Content = root;
        Render();
    }

    public IEnumerable? Decorations
    {
        get => (IEnumerable?)GetValue(DecorationsProperty);
        set => SetValue(DecorationsProperty, value);
    }

    public IList? SelectedDates
    {
        get => (IList?)GetValue(SelectedDatesProperty);
        set => SetValue(SelectedDatesProperty, value);
    }

    public bool HasSelectedDates
    {
        get => (bool)GetValue(HasSelectedDatesProperty);
        set => SetValue(HasSelectedDatesProperty, value);
    }

    public ICommand? DateClickedCommand
    {
        get => (ICommand?)GetValue(DateClickedCommandProperty);
        set => SetValue(DateClickedCommandProperty, value);
    }

    public DayOfWeek FirstDayOfWeek
    {
        get => (DayOfWeek)GetValue(FirstDayOfWeekProperty);
        set => SetValue(FirstDayOfWeekProperty, value);
    }

    private void NavigateMonth(int offset)
    {
        _displayMonth = _displayMonth.AddMonths(offset);
        Render();
    }

    private void Render()
    {
        _monthLabel.Text = _displayMonth.ToString("MMMM yyyy");
        _dayGrid.Children.Clear();
        _dayGrid.RowDefinitions.Clear();

        // Header row
        _dayGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        var dayNames = GetOrderedDayNames();
        for (var i = 0; i < 7; i++)
        {
            var headerLabel = new Label
            {
                Text = dayNames[i],
                FontSize = 11,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                Opacity = 0.6
            };
            Grid.SetColumn(headerLabel, i);
            Grid.SetRow(headerLabel, 0);
            _dayGrid.Children.Add(headerLabel);
        }

        // Build decoration lookup
        var decorationMap = BuildDecorationMap();

        // Calendar days
        var firstDay = _displayMonth;
        var daysInMonth = DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
        var startDayOfWeek = GetDayColumn(firstDay.DayOfWeek);

        var row = 1;
        var col = startDayOfWeek;

        // Add rows as needed (max 6 weeks + header)
        for (var r = 0; r < 6; r++)
            _dayGrid.RowDefinitions.Add(new RowDefinition(36));

        var today = DateOnly.FromDateTime(DateTime.Today);

        for (var day = 1; day <= daysInMonth; day++)
        {
            var date = new DateOnly(firstDay.Year, firstDay.Month, day);
            var cell = CreateDayCell(date, today, decorationMap);

            Grid.SetColumn(cell, col);
            Grid.SetRow(cell, row);
            _dayGrid.Children.Add(cell);

            col++;
            if (col > 6)
            {
                col = 0;
                row++;
            }
        }
    }

    private View CreateDayCell(DateOnly date, DateOnly today, Dictionary<DateOnly, CalendarDateDecoration> decorationMap)
    {
        var isSelected = _selectedDates.Contains(date);
        var isToday = date == today;
        var hasDecoration = decorationMap.TryGetValue(date, out var decoration);

        Color bgColor;
        Color textColor;

        if (isSelected)
        {
            bgColor = SelectedColor;
            textColor = Colors.White;
        }
        else if (hasDecoration)
        {
            bgColor = decoration!.Color.WithAlpha(0.3f);
            textColor = decoration.Color;
        }
        else if (isToday)
        {
            bgColor = TodayColor;
            textColor = Colors.Black;
        }
        else
        {
            bgColor = Colors.Transparent;
            textColor = Colors.Black;
        }

        var label = new Label
        {
            Text = date.Day.ToString(),
            FontSize = 13,
            FontAttributes = hasDecoration ? FontAttributes.Bold : FontAttributes.None,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            TextColor = textColor
        };

        var cell = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 6 },
            StrokeThickness = isSelected ? 2 : hasDecoration ? 1 : 0,
            Stroke = isSelected ? SelectedColor : hasDecoration ? decoration!.Color : Colors.Transparent,
            BackgroundColor = bgColor,
            Padding = 2,
            Content = label
        };

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (_, _) => OnDateTapped(date, hasDecoration);
        cell.GestureRecognizers.Add(tapGesture);

        return cell;
    }

    private void OnDateTapped(DateOnly date, bool hasDecoration)
    {
        if (hasDecoration)
        {
            // Clicking a decorated date fires the DateClickedCommand
            DateClickedCommand?.Execute(date.ToDateTime(TimeOnly.MinValue));
            return;
        }

        // Toggle selection for non-decorated dates (multi-select)
        if (!_selectedDates.Add(date))
        {
            _selectedDates.Remove(date);
        }

        SyncSelectedDates();
        Render();
    }

    private void SyncSelectedDates()
    {
        var list = SelectedDates;
        if (list is not null)
        {
            list.Clear();
            foreach (var date in _selectedDates.OrderBy(d => d))
                list.Add(date.ToDateTime(TimeOnly.MinValue));
        }

        HasSelectedDates = _selectedDates.Count > 0;
    }

    public void ClearSelection()
    {
        _selectedDates.Clear();
        SyncSelectedDates();
        Render();
    }

    private Dictionary<DateOnly, CalendarDateDecoration> BuildDecorationMap()
    {
        var map = new Dictionary<DateOnly, CalendarDateDecoration>();
        if (Decorations is null) return map;

        foreach (var item in Decorations)
        {
            if (item is CalendarDateDecoration decoration)
                map[decoration.Date] = decoration;
        }

        return map;
    }

    private string[] GetOrderedDayNames()
    {
        var names = new string[7];
        var first = (int)FirstDayOfWeek;
        for (var i = 0; i < 7; i++)
        {
            var dow = (DayOfWeek)((first + i) % 7);
            names[i] = dow.ToString()[..2];
        }

        return names;
    }

    private int GetDayColumn(DayOfWeek dayOfWeek)
    {
        var first = (int)FirstDayOfWeek;
        var day = (int)dayOfWeek;
        return (day - first + 7) % 7;
    }

    private static void OnDecorationsChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        var calendar = (CalendarView)bindable;

        if (oldValue is INotifyCollectionChanged oldCollection)
            oldCollection.CollectionChanged -= calendar.OnDecorationsCollectionChanged;

        if (newValue is INotifyCollectionChanged newCollection)
            newCollection.CollectionChanged += calendar.OnDecorationsCollectionChanged;

        calendar.Render();
    }

    private void OnDecorationsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Render();
    }
}
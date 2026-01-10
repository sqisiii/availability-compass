using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AvailabilityCompass.WpfClient.Shared.Controls;

/// <summary>
/// A control that displays Segoe Fluent Icons (Windows 11) or Segoe MDL2 Assets (Windows 10).
/// </summary>
public class FluentIcon : TextBlock
{
    static FluentIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(FluentIcon),
            new FrameworkPropertyMetadata(typeof(FluentIcon)));
    }

    public FluentIcon()
    {
        FontFamily = new FontFamily("Segoe Fluent Icons, Segoe MDL2 Assets");
        FontSize = 16;
        TextAlignment = TextAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        HorizontalAlignment = HorizontalAlignment.Center;
    }

    #region Glyph Property

    /// <summary>
    /// The Unicode glyph code to display (e.g., "\uE787" for Calendar).
    /// </summary>
    public static readonly DependencyProperty GlyphProperty =
        DependencyProperty.Register(
            nameof(Glyph),
            typeof(string),
            typeof(FluentIcon),
            new PropertyMetadata(string.Empty, OnGlyphChanged));

    public string Glyph
    {
        get => (string)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    private static void OnGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FluentIcon icon)
        {
            icon.Text = e.NewValue as string ?? string.Empty;
        }
    }

    #endregion

    #region IconSize Property

    /// <summary>
    /// The size of the icon in pixels.
    /// </summary>
    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register(
            nameof(IconSize),
            typeof(double),
            typeof(FluentIcon),
            new PropertyMetadata(16.0, OnIconSizeChanged));

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    private static void OnIconSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FluentIcon icon && e.NewValue is double size)
        {
            icon.FontSize = size;
            icon.Width = size;
            icon.Height = size;
        }
    }

    #endregion
}

/// <summary>
/// Common Fluent Icon glyph codes for easy reference.
/// </summary>
public static class FluentIcons
{
    // Navigation
    public const string Menu = "\uE700";
    public const string Back = "\uE72B";
    public const string Forward = "\uE72A";
    public const string Home = "\uE80F";
    public const string Settings = "\uE713";

    // Window controls
    public const string ChromeMinimize = "\uE921";
    public const string ChromeMaximize = "\uE922";
    public const string ChromeRestore = "\uE923";
    public const string ChromeClose = "\uE8BB";
    public const string Close = "\uE8BB";

    // Actions
    public const string Add = "\uE710";
    public const string Edit = "\uE70F";
    public const string Delete = "\uE74D";
    public const string Save = "\uE74E";
    public const string Refresh = "\uE72C";
    public const string Search = "\uE721";
    public const string Filter = "\uE71C";
    public const string Sort = "\uE8CB";
    public const string Copy = "\uE8C8";
    public const string Paste = "\uE77F";
    public const string Undo = "\uE7A7";
    public const string Redo = "\uE7A6";

    // Status
    public const string CheckMark = "\uE73E";
    public const string Cancel = "\uE711";
    public const string Warning = "\uE7BA";
    public const string Error = "\uE783";
    public const string Info = "\uE946";

    // Theme
    public const string Brightness = "\uE706";
    public const string ClearNight = "\uE708";
    public const string WeatherSunny = "\uE706";
    public const string WeatherMoon = "\uE708";

    // Data
    public const string Calendar = "\uE787";
    public const string CalendarDay = "\uE8BF";
    public const string Database = "\uE8F1";
    public const string Folder = "\uE8B7";
    public const string Document = "\uE8A5";
    public const string Link = "\uE71B";
    public const string Location = "\uE81D";

    // Arrows
    public const string ChevronUp = "\uE70E";
    public const string ChevronDown = "\uE70D";
    public const string ChevronLeft = "\uE76B";
    public const string ChevronRight = "\uE76C";
    public const string ArrowUp = "\uE74A";
    public const string ArrowDown = "\uE74B";

    // Misc
    public const string MoreVertical = "\uE712";
    public const string MoreHorizontal = "\uE10C";
    public const string Pin = "\uE718";
    public const string Unpin = "\uE77A";
    public const string Favorite = "\uE734";
    public const string FavoriteFilled = "\uE735";
    public const string Share = "\uE72D";
    public const string Download = "\uE896";
    public const string Upload = "\uE898";

    // Calendar types
    public const string Accept = "\uE8FB"; // Checkmark in circle (for Include calendars)
    public const string Blocked = "\uE8D8"; // Prohibited sign (for Exclude calendars)
}
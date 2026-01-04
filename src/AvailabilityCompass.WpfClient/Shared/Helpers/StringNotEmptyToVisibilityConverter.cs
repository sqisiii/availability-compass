using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AvailabilityCompass.WpfClient.Shared.Helpers;

/// <summary>
/// Converts a non-empty string to Visible, and empty/null string to Collapsed.
/// </summary>
public class StringNotEmptyToVisibilityConverter : IValueConverter
{
    private static readonly object BoxedVisible = Visibility.Visible;
    private static readonly object BoxedCollapsed = Visibility.Collapsed;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string str && !string.IsNullOrEmpty(str)
            ? BoxedVisible
            : BoxedCollapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

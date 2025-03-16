using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AvailabilityCompass.WpfClient.Shared.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    // ReSharper disable once MemberCanBePrivate.Global
    public bool Invert { get; set; } = false;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            return Visibility.Collapsed; // Default if value is not a boolean
        }

        if (Invert)
        {
            boolValue = !boolValue;
        }

        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return Invert ? visibility != Visibility.Visible : visibility == Visibility.Visible;
        }

        return false;
    }
}
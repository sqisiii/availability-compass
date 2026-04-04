using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AvailabilityCompass.WpfClient.Shared.Converters;

/// <summary>
/// Converts an enum value to Visibility by comparing it against the ConverterParameter.
/// Visible when the value equals the parameter, Collapsed otherwise.
/// </summary>
public class EnumToVisibilityConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null || parameter is null)
        {
            return Visibility.Collapsed;
        }

        return value.ToString() == parameter.ToString() ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
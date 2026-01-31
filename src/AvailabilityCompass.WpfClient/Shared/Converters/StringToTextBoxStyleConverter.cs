using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AvailabilityCompass.WpfClient.Shared.Converters;

/// <summary>
/// Converts a string (error message) to a TextBox style.
/// Returns GlassTextBoxError if string has value, GlassTextBox otherwise.
/// </summary>
public class StringToTextBoxStyleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var hasError = value is string str && !string.IsNullOrEmpty(str);
        var styleKey = hasError ? "GlassTextBoxError" : "GlassTextBox";
        return System.Windows.Application.Current.FindResource(styleKey);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

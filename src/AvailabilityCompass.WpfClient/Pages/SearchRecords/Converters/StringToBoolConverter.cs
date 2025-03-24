using System.Globalization;
using System.Windows.Data;

namespace AvailabilityCompass.WpfClient.Pages.SearchRecords.Converters;

/// <summary>
/// Converts a string (true/false) to a boolean value and vice versa.
/// </summary>
public class StringToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string strValue && string.Equals(strValue, "True", StringComparison.CurrentCultureIgnoreCase);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            return "N/A";
        }

        return boolValue ? "True" : "False"; // Default conversion
    }
}
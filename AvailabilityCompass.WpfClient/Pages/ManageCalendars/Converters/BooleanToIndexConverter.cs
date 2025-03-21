using System.Globalization;
using System.Windows.Data;

namespace AvailabilityCompass.WpfClient.Pages.ManageCalendars.Converters;

public class BooleanToIndexConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? 1 : 0;
        }

        return 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index)
        {
            return index == 1;
        }

        return false;
    }
}
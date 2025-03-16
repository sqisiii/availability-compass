using System.Globalization;
using System.Windows.Data;

namespace AvailabilityCompass.WpfClient.Pages.ManageSources.Converters;

public class DoubleToPercentageConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double doubleValue)
        {
            return "Idle";
        }

        var doubleTolerance = 0.0001;
        return doubleValue == 0 ? "Idle" : Math.Abs(doubleValue - 100) < doubleTolerance ? "Completed" : $"Processing: {doubleValue:0}%";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string strValue)
        {
            return 0.0;
        }

        return double.TryParse(strValue.Replace("Progress ", "").Replace("%", ""), out double result) ? result : 0.0;
    }
}
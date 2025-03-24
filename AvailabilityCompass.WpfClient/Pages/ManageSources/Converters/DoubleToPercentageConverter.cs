using System.Globalization;
using System.Windows.Data;

namespace AvailabilityCompass.WpfClient.Pages.ManageSources.Converters;

/// <summary>
/// Converts a double value to a percentage string representation and vice versa.
/// </summary>
public class DoubleToPercentageConverter : IValueConverter
{
    private readonly object _defaultPercentValue = 0.0;
    private readonly double doubleTolerance = 0.0001;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double doubleValue)
        {
            return "Idle";
        }

        return doubleValue == 0 ? "Idle" : Math.Abs(doubleValue - 100) < doubleTolerance ? "Completed" : $"Processing: {doubleValue:0}%";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string strValue)
        {
            return _defaultPercentValue;
        }

        return double.TryParse(strValue.Replace("Progress ", "").Replace("%", ""), out double result) ? result : _defaultPercentValue;
    }
}
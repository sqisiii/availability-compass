using System.Globalization;

namespace AvailabilityCompass.MauiClient.Shared.Converters;

public class DoubleToPercentageConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is double d)
            return $"{d:F0}%";
        return "0%";
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
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AvailabilityCompass.WpfClient.Pages.SearchRecords.Converters;

/// <summary>
/// Converts calendar IsOnly boolean to appropriate icon color.
/// IsOnly=true (Include) -> GlassSuccessBrush (green)
/// IsOnly=false (Exclude) -> GlassPrimaryBrush (amber)
/// </summary>
public class CalendarTypeToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isOnly)
        {
            var brushKey = isOnly ? "GlassSuccessBrush" : "GlassPrimaryBrush";
            return System.Windows.Application.Current.TryFindResource(brushKey) as Brush;
        }

        return System.Windows.Application.Current.TryFindResource("GlassPrimaryBrush") as Brush;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

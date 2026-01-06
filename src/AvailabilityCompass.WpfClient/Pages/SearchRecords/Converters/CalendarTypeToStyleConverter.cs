using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AvailabilityCompass.WpfClient.Pages.SearchRecords.Converters;

/// <summary>
/// Converts calendar IsOnly boolean to appropriate floating card style.
/// IsOnly=true (Include) -> GlassFloatingCardInclude
/// IsOnly=false (Exclude) -> GlassFloatingCardExclude
/// </summary>
public class CalendarTypeToStyleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isOnly)
        {
            var styleKey = isOnly ? "GlassFloatingCardInclude" : "GlassFloatingCardExclude";
            return System.Windows.Application.Current.TryFindResource(styleKey) as Style;
        }

        return System.Windows.Application.Current.TryFindResource("GlassFloatingCard") as Style;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

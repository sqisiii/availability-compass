using System.Globalization;
using System.Windows.Data;
using AvailabilityCompass.WpfClient.Shared.Controls;

namespace AvailabilityCompass.WpfClient.Pages.SearchRecords.Converters;

/// <summary>
/// Converts calendar IsOnly boolean to the appropriate icon glyph.
/// IsOnly=true (Include) -> Accept icon
/// IsOnly=false (Exclude) -> Blocked icon
/// </summary>
public class CalendarTypeToIconConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is bool isOnly)
        {
            return isOnly ? FluentIcons.Accept : FluentIcons.Blocked;
        }

        return FluentIcons.Calendar;
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
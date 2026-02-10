using System.Globalization;
using System.Windows.Data;
using AvailabilityCompass.Core.Features.SearchRecords;

namespace AvailabilityCompass.WpfClient.Pages.SearchRecords.Converters;

/// <summary>
/// Extracts calendar overlap summaries from a result dictionary.
/// </summary>
public class DictionaryToCalendarOverlapsConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not Dictionary<string, object> dict)
        {
            return null;
        }

        return dict.TryGetValue("CalendarOverlaps", out var overlaps)
            ? overlaps as IReadOnlyCollection<CalendarOverlapSummary>
            : null;
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
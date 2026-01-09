using System.Globalization;
using System.Windows.Data;

namespace AvailabilityCompass.WpfClient.Pages.SearchRecords.Converters;

/// <summary>
/// Extracts additional fields from a Dictionary (excluding core fields like SourceName, Title, Url, StartDate, EndDate)
/// and returns them as a list of KeyValuePairs for display.
/// </summary>
public class DictionaryToAdditionalFieldsConverter : IValueConverter
{
    private static readonly HashSet<string> CoreFields =
    [
        "SourceName",
        "Title",
        "Url",
        "StartDate",
        "EndDate"
    ];

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

        return dict
            .Where(kvp => !CoreFields.Contains(kvp.Key) && !string.IsNullOrEmpty(kvp.Value.ToString()))
            .Select(kvp => new KeyValuePair<string, string>(FormatKey(kvp.Key), kvp.Value.ToString() ?? ""))
            .ToList();
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static string FormatKey(string key)
    {
        // Add space before capital letters (e.g., "ShipName" -> "Ship Name")
        var result = string.Concat(key.Select((c, i) => i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
        return result;
    }
}
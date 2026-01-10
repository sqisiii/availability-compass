using System.Globalization;
using System.Windows.Data;

namespace AvailabilityCompass.WpfClient.Pages.SearchRecords.Converters;

/// <summary>
/// Converts container width to card width ensuring exactly CardsPerRow cards fit per row.
/// </summary>
public class ContainerWidthToCardWidthConverter : IValueConverter
{
    public int CardsPerRow { get; set; } = 5;
    public double CardMargin { get; set; } = 8;
    public double MinCardWidth { get; set; } = 150;
    public double ScrollBarWidth { get; set; } = 20;

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not double containerWidth || !(containerWidth > 0))
        {
            return 220;
        }

        // Account for scrollbar and card margins
        var availableWidth = containerWidth - ScrollBarWidth;
        var totalMargins = CardsPerRow * CardMargin * 2;
        availableWidth -= totalMargins;
        var calculatedWidth = availableWidth / CardsPerRow;
        return Math.Max(MinCardWidth, calculatedWidth);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
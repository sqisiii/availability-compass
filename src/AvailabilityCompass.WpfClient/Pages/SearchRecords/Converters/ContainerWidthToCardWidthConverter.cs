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

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not double containerWidth || containerWidth <= 0)
        {
            return 220;
        }

        // Scrollbar is 8px (from GlassControls.xaml template), add small buffer
        const double scrollBarWidth = 12;

        // Each card has Margin="8" which means 8px on each side = 16px total per card
        var marginPerCard = CardMargin * 2;

        // Available width after scrollbar
        var availableWidth = containerWidth - scrollBarWidth;

        // Total margins for all cards
        var totalMargins = CardsPerRow * marginPerCard;

        // Width available for card content (excluding margins)
        var contentWidth = availableWidth - totalMargins;

        // Use floor to ensure cards definitely fit (avoid floating-point overflow)
        var calculatedWidth = Math.Floor(contentWidth / CardsPerRow);

        // If calculated width is below minimum, reduce cards per row conceptually
        // but still return at least MinCardWidth (let fewer cards show)
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
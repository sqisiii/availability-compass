using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AvailabilityCompass.Core.Features.Search.FilterFormElements;

namespace AvailabilityCompass.WpfClient.Pages.SearchRecords.Converters;

public class FormGroupVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is FormElementType elementType && parameter is string targetTypeString)
        {
            return elementType.ToString() == targetTypeString ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //skipping converting back intentionally
        throw new NotImplementedException();
    }
}
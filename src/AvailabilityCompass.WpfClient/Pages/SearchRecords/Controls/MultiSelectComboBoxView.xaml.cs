using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;

namespace AvailabilityCompass.WpfClient.Pages.SearchRecords.Controls;

public partial class MultiSelectComboBoxView : UserControl
{
    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register(nameof(ItemSource), typeof(IEnumerable<FormElementSelectOption>), typeof(MultiSelectComboBoxView), new PropertyMetadata(default(IEnumerable)));

    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
        nameof(SelectedItems), typeof(IList<string>), typeof(MultiSelectComboBoxView), new PropertyMetadata(default(IList<string>)));

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label), typeof(string), typeof(MultiSelectComboBoxView), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty DisplayValueProperty = DependencyProperty.Register(
        nameof(DisplayValue), typeof(string), typeof(MultiSelectComboBoxView), new PropertyMetadata(null, DisplayValuePropertyChanged));

    public MultiSelectComboBoxView()
    {
        InitializeComponent();
    }

    public string DisplayValue
    {
        get => (string)GetValue(DisplayValueProperty);
        set => SetValue(DisplayValueProperty, value);
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public IEnumerable<FormElementSelectOption> ItemSource
    {
        get => (IEnumerable<FormElementSelectOption>)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    public IList<string>? SelectedItems
    {
        get => (IList<string>?)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    private static void DisplayValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not string value)
        {
            return;
        }

        if (!string.IsNullOrEmpty(value))
        {
            return;
        }

        if (d is not MultiSelectComboBoxView multiSelectComboBoxView)
        {
            return;
        }

        multiSelectComboBoxView.SelectedItems?.Clear();
        foreach (var option in multiSelectComboBoxView.ItemSource)
        {
            option.IsSelected = false;
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox checkBox)
        {
            return;
        }

        if (SelectedItems is null)
        {
            return;
        }

        var checkBoxContent = checkBox.Content?.ToString();
        if (string.IsNullOrEmpty(checkBoxContent))
        {
            return;
        }

        if (checkBox.IsChecked == true)
        {
            SelectedItems.Add(checkBoxContent);
        }
        else
        {
            SelectedItems.Remove(checkBoxContent);
        }

        DisplayValue = CreateLimitedString(SelectedItems);
    }

    private string CreateLimitedString(IEnumerable<string> options, int maxLength = 50)
    {
        var enumerable = options.ToList();
        if (!enumerable.Any())
        {
            return string.Empty;
        }

        var result = new StringBuilder();
        var currentLength = 0;
        var truncated = false;

        foreach (var option in enumerable)
        {
            var additionalLength = option.Length + (currentLength > 0 ? 1 : 0);
            if (currentLength + additionalLength > maxLength)
            {
                truncated = true;
                break;
            }

            if (currentLength > 0)
                result.Append(", ");

            result.Append(option);
            currentLength += additionalLength;
        }

        if (truncated && result.Length + 3 <= maxLength)
            result.Append("...");

        return result.ToString();
    }

    private void MultiSelectComboBoxView_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (SelectedItems != null)
        {
            DisplayValue = CreateLimitedString(SelectedItems);
        }
    }
}
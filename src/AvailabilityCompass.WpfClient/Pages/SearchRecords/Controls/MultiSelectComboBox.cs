using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AvailabilityCompass.WpfClient.Pages.SearchRecords.Controls;

/// <summary>
/// A custom ComboBox control that supports multiple selection.
/// </summary>
/// <remarks>
/// SelectedValues property in XAML ComboBox is readonly collection, so it is not possible to bind it to a list of selected items.
/// </remarks>
public sealed class MultiSelectComboBox : ComboBox
{
    public static readonly DependencyProperty SelectedValuesProperty = DependencyProperty.Register(
        nameof(SelectedValues), typeof(IList), typeof(MultiSelectComboBox), new PropertyMetadata(default(IList)));

    public static readonly DependencyProperty DisplayValueProperty = DependencyProperty.Register(
        nameof(DisplayValue), typeof(string), typeof(MultiSelectComboBox), new PropertyMetadata(default(string)));

    public static readonly RoutedCommand ClearCommand = new("Clear", typeof(MultiSelectComboBox));

    static MultiSelectComboBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectComboBox),
            new FrameworkPropertyMetadata(typeof(ComboBox)));
    }

    public MultiSelectComboBox()
    {
        CommandBindings.Add(new CommandBinding(ClearCommand, OnClearCommand, CanExecuteClearCommand));
    }

    public string DisplayValue
    {
        get => (string)GetValue(DisplayValueProperty);
        set => SetValue(DisplayValueProperty, value);
    }

    public IList? SelectedValues
    {
        get => (IList?)GetValue(SelectedValuesProperty);
        set => SetValue(SelectedValuesProperty, value);
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        //required so clicking on the element on the list doesn't trigger the selection
        return new ContentControl();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        //required so clicking on the element on the list doesn't trigger the selection
        return item is UIElement;
    }

    private static void CanExecuteClearCommand(object sender, CanExecuteRoutedEventArgs e)
    {
        if (sender is MultiSelectComboBox comboBox)
        {
            e.CanExecute = !string.IsNullOrEmpty(comboBox.DisplayValue);
        }
    }

    private static void OnClearCommand(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is not MultiSelectComboBox multiSelectComboBox)
        {
            return;
        }

        multiSelectComboBox.SelectedValues?.Clear();
        multiSelectComboBox.DisplayValue = string.Empty;
        e.Handled = true;
    }
}

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf.Internal;

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

    static MultiSelectComboBox()
    {
        //required for the delete selection button to work
        ClearText.HandlesClearCommandProperty.OverrideMetadata(typeof(MultiSelectComboBox), new FrameworkPropertyMetadata(OnHandlesClearCommandChanged));
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

    private static void OnHandlesClearCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MultiSelectComboBox element)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            RemoveClearTextCommand(element);
            element.CommandBindings.Add(new CommandBinding(ClearText.ClearCommand, OnClearCommand));
        }
        else
        {
            RemoveClearTextCommand(element);
        }

        return;

        static void RemoveClearTextCommand(MultiSelectComboBox element)
        {
            for (var i = element.CommandBindings.Count - 1; i >= 0; i--)
            {
                if (element.CommandBindings[i].Command == ClearText.ClearCommand)
                {
                    element.CommandBindings.RemoveAt(i);
                }
            }
        }

        static void OnClearCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (sender is not MultiSelectComboBox multiSelectComboBox)
            {
                return;
            }

            multiSelectComboBox.SelectedValues?.Clear();
            multiSelectComboBox.DisplayValue = string.Empty;
            eventArgs.Handled = true;
        }
    }
}
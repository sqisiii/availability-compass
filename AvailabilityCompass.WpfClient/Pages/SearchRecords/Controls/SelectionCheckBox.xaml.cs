using System.Windows;
using System.Windows.Controls;

namespace AvailabilityCompass.WpfClient.Pages.SearchRecords.Controls;

public partial class SelectionCheckBox : UserControl
{
    public static readonly DependencyProperty HeaderTextProperty =
        DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(SelectionCheckBox), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty CheckBoxTextProperty =
        DependencyProperty.Register(nameof(CheckBoxText), typeof(string), typeof(SelectionCheckBox), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(nameof(IsChecked), typeof(bool?), typeof(SelectionCheckBox), new PropertyMetadata(default(bool?)));

    public static readonly DependencyProperty FullMinHeightProperty =
        DependencyProperty.Register(nameof(FullMinHeight), typeof(double), typeof(SelectionCheckBox), new PropertyMetadata(0d));

    public static readonly DependencyProperty FullWidthProperty =
        DependencyProperty.Register(nameof(FullWidth), typeof(double), typeof(SelectionCheckBox), new PropertyMetadata(0d));

    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive), typeof(bool), typeof(SelectionCheckBox), new PropertyMetadata(false));

    public SelectionCheckBox()
    {
        InitializeComponent();
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public string CheckBoxText
    {
        get => (string)GetValue(CheckBoxTextProperty);
        set => SetValue(CheckBoxTextProperty, value);
    }

    public bool? IsChecked
    {
        get => (bool?)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public double FullMinHeight
    {
        get => (double)GetValue(FullMinHeightProperty);
        set => SetValue(FullMinHeightProperty, value);
    }

    public double FullWidth
    {
        get => (double)GetValue(FullWidthProperty);
        set => SetValue(FullWidthProperty, value);
    }
}
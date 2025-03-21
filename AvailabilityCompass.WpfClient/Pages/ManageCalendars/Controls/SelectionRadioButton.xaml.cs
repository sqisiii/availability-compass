using System.Windows;
using System.Windows.Controls;

namespace AvailabilityCompass.WpfClient.Pages.ManageCalendars.Controls;

public partial class SelectionRadioButton : UserControl
{
    public static readonly DependencyProperty HeaderTextProperty =
        DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(SelectionRadioButton), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty RadioButtonTextProperty =
        DependencyProperty.Register(nameof(RadioButtonText), typeof(string), typeof(SelectionRadioButton), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(nameof(IsChecked), typeof(bool?), typeof(SelectionRadioButton), new PropertyMetadata(default(bool?)));

    public static readonly DependencyProperty FullMinHeightProperty =
        DependencyProperty.Register(nameof(FullMinHeight), typeof(double), typeof(SelectionRadioButton), new PropertyMetadata(0d));

    public static readonly DependencyProperty FullWidthProperty =
        DependencyProperty.Register(nameof(FullWidth), typeof(double), typeof(SelectionRadioButton), new PropertyMetadata(0d));

    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive), typeof(bool), typeof(SelectionRadioButton), new PropertyMetadata(false));

    public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(
        nameof(GroupName), typeof(string), typeof(SelectionRadioButton), new PropertyMetadata(default(string)));

    public SelectionRadioButton()
    {
        InitializeComponent();
    }

    public string GroupName
    {
        get => (string)GetValue(GroupNameProperty);
        set => SetValue(GroupNameProperty, value);
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

    public string RadioButtonText
    {
        get => (string)GetValue(RadioButtonTextProperty);
        set => SetValue(RadioButtonTextProperty, value);
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
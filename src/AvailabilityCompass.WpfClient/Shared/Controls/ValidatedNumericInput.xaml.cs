using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AvailabilityCompass.WpfClient.Shared.Controls;

public partial class ValidatedNumericInput : UserControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(ValidatedNumericInput),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(int?),
            typeof(ValidatedNumericInput),
            new FrameworkPropertyMetadata(
                default(int?),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty ValidationErrorProperty =
        DependencyProperty.Register(
            nameof(ValidationError),
            typeof(string),
            typeof(ValidatedNumericInput),
            new PropertyMetadata(default(string)));

    public ValidatedNumericInput()
    {
        InitializeComponent();
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public int? Value
    {
        get => (int?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string? ValidationError
    {
        get => (string?)GetValue(ValidationErrorProperty);
        set => SetValue(ValidationErrorProperty, value);
    }

    private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !int.TryParse(e.Text, out _);
    }
}

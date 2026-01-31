using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace AvailabilityCompass.WpfClient.Shared.Controls;

[ContentProperty(nameof(DialogContent))]
public partial class ModalOverlay : UserControl
{
    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(
            nameof(IsOpen),
            typeof(bool),
            typeof(ModalOverlay),
            new PropertyMetadata(false));

    public static readonly DependencyProperty DialogContentProperty =
        DependencyProperty.Register(
            nameof(DialogContent),
            typeof(object),
            typeof(ModalOverlay),
            new PropertyMetadata(null));

    public static readonly DependencyProperty MinDialogWidthProperty =
        DependencyProperty.Register(
            nameof(MinDialogWidth),
            typeof(double),
            typeof(ModalOverlay),
            new PropertyMetadata(300.0));

    public static readonly DependencyProperty MaxDialogWidthProperty =
        DependencyProperty.Register(
            nameof(MaxDialogWidth),
            typeof(double),
            typeof(ModalOverlay),
            new PropertyMetadata(400.0));

    public ModalOverlay()
    {
        InitializeComponent();
    }

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public object? DialogContent
    {
        get => GetValue(DialogContentProperty);
        set => SetValue(DialogContentProperty, value);
    }

    public double MinDialogWidth
    {
        get => (double)GetValue(MinDialogWidthProperty);
        set => SetValue(MinDialogWidthProperty, value);
    }

    public double MaxDialogWidth
    {
        get => (double)GetValue(MaxDialogWidthProperty);
        set => SetValue(MaxDialogWidthProperty, value);
    }
}

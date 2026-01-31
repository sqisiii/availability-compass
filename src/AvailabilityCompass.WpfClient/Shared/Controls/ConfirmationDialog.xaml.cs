using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AvailabilityCompass.WpfClient.Shared.Controls;

public partial class ConfirmationDialog : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(ConfirmationDialog),
            new PropertyMetadata("Confirm"));

    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(
            nameof(Message),
            typeof(string),
            typeof(ConfirmationDialog),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ConfirmTextProperty =
        DependencyProperty.Register(
            nameof(ConfirmText),
            typeof(string),
            typeof(ConfirmationDialog),
            new PropertyMetadata("Confirm"));

    public static readonly DependencyProperty CancelTextProperty =
        DependencyProperty.Register(
            nameof(CancelText),
            typeof(string),
            typeof(ConfirmationDialog),
            new PropertyMetadata("Cancel"));

    public static readonly DependencyProperty ConfirmCommandProperty =
        DependencyProperty.Register(
            nameof(ConfirmCommand),
            typeof(ICommand),
            typeof(ConfirmationDialog),
            new PropertyMetadata(null));

    public static readonly DependencyProperty CancelCommandProperty =
        DependencyProperty.Register(
            nameof(CancelCommand),
            typeof(ICommand),
            typeof(ConfirmationDialog),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IsDestructiveProperty =
        DependencyProperty.Register(
            nameof(IsDestructive),
            typeof(bool),
            typeof(ConfirmationDialog),
            new PropertyMetadata(false, OnIsDestructiveChanged));

    private static readonly DependencyPropertyKey ConfirmButtonStylePropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ConfirmButtonStyle),
            typeof(Style),
            typeof(ConfirmationDialog),
            new PropertyMetadata(null));

    public static readonly DependencyProperty ConfirmButtonStyleProperty =
        ConfirmButtonStylePropertyKey.DependencyProperty;

    public ConfirmationDialog()
    {
        InitializeComponent();
        UpdateConfirmButtonStyle();
    }

    private static void OnIsDestructiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ConfirmationDialog)d).UpdateConfirmButtonStyle();
    }

    private void UpdateConfirmButtonStyle()
    {
        var styleKey = IsDestructive ? "GlassDestructiveButton" : "GlassPrimaryButton";
        ConfirmButtonStyle = (Style)System.Windows.Application.Current.FindResource(styleKey);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public string ConfirmText
    {
        get => (string)GetValue(ConfirmTextProperty);
        set => SetValue(ConfirmTextProperty, value);
    }

    public string CancelText
    {
        get => (string)GetValue(CancelTextProperty);
        set => SetValue(CancelTextProperty, value);
    }

    public ICommand? ConfirmCommand
    {
        get => (ICommand?)GetValue(ConfirmCommandProperty);
        set => SetValue(ConfirmCommandProperty, value);
    }

    public ICommand? CancelCommand
    {
        get => (ICommand?)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    public bool IsDestructive
    {
        get => (bool)GetValue(IsDestructiveProperty);
        set => SetValue(IsDestructiveProperty, value);
    }

    public Style? ConfirmButtonStyle
    {
        get => (Style?)GetValue(ConfirmButtonStyleProperty);
        private set => SetValue(ConfirmButtonStylePropertyKey, value);
    }
}

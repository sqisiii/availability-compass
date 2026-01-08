using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AvailabilityCompass.WpfClient.Shared.Controls;

public class CollapsibleSection : ContentControl
{
    static CollapsibleSection()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CollapsibleSection),
            new FrameworkPropertyMetadata(typeof(CollapsibleSection)));
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(CollapsibleSection),
            new PropertyMetadata(string.Empty));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty SummaryProperty =
        DependencyProperty.Register(
            nameof(Summary),
            typeof(string),
            typeof(CollapsibleSection),
            new PropertyMetadata(string.Empty));

    public string Summary
    {
        get => (string)GetValue(SummaryProperty);
        set => SetValue(SummaryProperty, value);
    }

    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register(
            nameof(IsExpanded),
            typeof(bool),
            typeof(CollapsibleSection),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public static readonly DependencyProperty IconBrushProperty =
        DependencyProperty.Register(
            nameof(IconBrush),
            typeof(Brush),
            typeof(CollapsibleSection),
            new PropertyMetadata(null));

    public Brush? IconBrush
    {
        get => (Brush?)GetValue(IconBrushProperty);
        set => SetValue(IconBrushProperty, value);
    }

    public static readonly DependencyProperty HasSelectionProperty =
        DependencyProperty.Register(
            nameof(HasSelection),
            typeof(bool),
            typeof(CollapsibleSection),
            new PropertyMetadata(false));

    public bool HasSelection
    {
        get => (bool)GetValue(HasSelectionProperty);
        set => SetValue(HasSelectionProperty, value);
    }
}

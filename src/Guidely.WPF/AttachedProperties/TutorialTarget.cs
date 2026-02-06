using System.Windows;

// ReSharper disable once CheckNamespace
namespace Guidely.WPF;

/// <summary>
/// Attached property for marking UI elements as tutorial targets.
/// </summary>
public static class TutorialTarget
{
    /// <summary>
    /// Identifies the Element attached property.
    /// </summary>
    public static readonly DependencyProperty ElementProperty =
        DependencyProperty.RegisterAttached(
            "Element",
            typeof(string),
            typeof(TutorialTarget),
            new PropertyMetadata(null, OnElementChanged));

    /// <summary>
    /// Gets the tutorial target element name for a dependency object.
    /// </summary>
    public static string? GetElement(DependencyObject obj) => (string?)obj.GetValue(ElementProperty);

    /// <summary>
    /// Sets the tutorial target element name for a dependency object.
    /// </summary>
    public static void SetElement(DependencyObject obj, string? value) => obj.SetValue(ElementProperty, value);

    private static void OnElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element)
        {
            return;
        }

        var oldValue = e.OldValue as string;
        var newValue = e.NewValue as string;

        // Unregister old value if it was set
        if (!string.IsNullOrEmpty(oldValue))
        {
            element.Loaded -= OnElementLoaded;
            element.Unloaded -= OnElementUnloaded;
            element.SizeChanged -= OnElementSizeChanged;
            TutorialElementRegistry.Unregister(oldValue, element);
        }

        // Register new value
        if (string.IsNullOrEmpty(newValue))
        {
            return;
        }

        element.Loaded += OnElementLoaded;
        element.Unloaded += OnElementUnloaded;

        // If already loaded, register immediately
        if (!element.IsLoaded)
        {
            return;
        }

        TutorialElementRegistry.Register(newValue, element);
        element.SizeChanged += OnElementSizeChanged;
    }

    private static void OnElementLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }

        var targetName = GetElement(element);
        if (string.IsNullOrEmpty(targetName))
        {
            return;
        }

        TutorialElementRegistry.Register(targetName, element);
        element.SizeChanged += OnElementSizeChanged;
    }

    private static void OnElementUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }

        var targetName = GetElement(element);
        if (string.IsNullOrEmpty(targetName))
        {
            return;
        }

        element.SizeChanged -= OnElementSizeChanged;
        TutorialElementRegistry.Unregister(targetName, element);
    }

    private static void OnElementSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }

        var targetName = GetElement(element);
        if (!string.IsNullOrEmpty(targetName) && element.IsLoaded)
        {
            // Re-register to fire ElementChanged and refresh cutouts/highlights
            TutorialElementRegistry.Register(targetName, element);
        }
    }
}
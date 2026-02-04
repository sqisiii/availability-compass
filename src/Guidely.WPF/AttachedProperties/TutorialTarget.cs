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
    public static string? GetElement(DependencyObject obj)
        => (string?)obj.GetValue(ElementProperty);

    /// <summary>
    /// Sets the tutorial target element name for a dependency object.
    /// </summary>
    public static void SetElement(DependencyObject obj, string? value)
        => obj.SetValue(ElementProperty, value);

    private static void OnElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element)
            return;

        var oldValue = e.OldValue as string;
        var newValue = e.NewValue as string;

        // Unregister old value if it was set
        if (!string.IsNullOrEmpty(oldValue))
        {
            element.Loaded -= OnElementLoaded;
            element.Unloaded -= OnElementUnloaded;
            TutorialElementRegistry.Unregister(oldValue);
        }

        // Register new value
        if (string.IsNullOrEmpty(newValue))
        {
            return;
        }

        element.Loaded += OnElementLoaded;
        element.Unloaded += OnElementUnloaded;

        // If already loaded, register immediately
        if (element.IsLoaded)
        {
            TutorialElementRegistry.Register(newValue, element);
        }
    }

    private static void OnElementLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
            return;

        var targetName = GetElement(element);
        if (!string.IsNullOrEmpty(targetName))
        {
            TutorialElementRegistry.Register(targetName, element);
        }
    }

    private static void OnElementUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
            return;

        var targetName = GetElement(element);
        if (!string.IsNullOrEmpty(targetName))
        {
            TutorialElementRegistry.Unregister(targetName);
        }
    }
}
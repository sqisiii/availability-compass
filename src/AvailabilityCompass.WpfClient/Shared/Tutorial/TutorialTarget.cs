using System.Windows;
using AvailabilityCompass.Core.Features.Tutorial;

namespace AvailabilityCompass.WpfClient.Shared.Tutorial;

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
            typeof(TutorialTargetElement),
            typeof(TutorialTarget),
            new PropertyMetadata(TutorialTargetElement.None, OnElementChanged));

    /// <summary>
    /// Gets the tutorial target element type for a dependency object.
    /// </summary>
    public static TutorialTargetElement GetElement(DependencyObject obj)
        => (TutorialTargetElement)obj.GetValue(ElementProperty);

    /// <summary>
    /// Sets the tutorial target element type for a dependency object.
    /// </summary>
    public static void SetElement(DependencyObject obj, TutorialTargetElement value)
        => obj.SetValue(ElementProperty, value);

    private static void OnElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element)
            return;

        var oldValue = (TutorialTargetElement)e.OldValue;
        var newValue = (TutorialTargetElement)e.NewValue;

        // Unregister an old value if it was set
        if (oldValue != TutorialTargetElement.None)
        {
            element.Loaded -= OnElementLoaded;
            element.Unloaded -= OnElementUnloaded;
            TutorialElementRegistry.Unregister(oldValue);
        }

        // Register new value
        if (newValue == TutorialTargetElement.None)
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

        var targetType = GetElement(element);
        if (targetType != TutorialTargetElement.None)
        {
            TutorialElementRegistry.Register(targetType, element);
        }
    }

    private static void OnElementUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
            return;

        var targetType = GetElement(element);
        if (targetType != TutorialTargetElement.None)
        {
            TutorialElementRegistry.Unregister(targetType);
        }
    }
}
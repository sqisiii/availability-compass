using System.Collections.Concurrent;
using System.Windows;
using AvailabilityCompass.Core.Features.Tutorial;

namespace AvailabilityCompass.WpfClient.Shared.Tutorial;

/// <summary>
/// Registry tracking UI elements marked as tutorial targets.
/// Uses weak references to avoid memory leaks when elements are unloaded.
/// </summary>
public static class TutorialElementRegistry
{
    private static readonly ConcurrentDictionary<TutorialTargetElement, WeakReference<FrameworkElement>> _elements = new();

    /// <summary>
    /// Raised when an element is registered or unregistered.
    /// </summary>
    public static event EventHandler<TutorialTargetElement>? ElementChanged;

    /// <summary>
    /// Registers a framework element as a tutorial target.
    /// </summary>
    public static void Register(TutorialTargetElement targetType, FrameworkElement element)
    {
        _elements[targetType] = new WeakReference<FrameworkElement>(element);
        ElementChanged?.Invoke(null, targetType);
    }

    /// <summary>
    /// Unregisters a framework element from the registry.
    /// </summary>
    public static void Unregister(TutorialTargetElement targetType)
    {
        _elements.TryRemove(targetType, out _);
        ElementChanged?.Invoke(null, targetType);
    }

    /// <summary>
    /// Gets the element for a tutorial target, or null if not registered or garbage collected.
    /// </summary>
    public static FrameworkElement? GetElement(TutorialTargetElement targetType)
    {
        if (_elements.TryGetValue(targetType, out var weakRef) &&
            weakRef.TryGetTarget(out var element))
        {
            return element;
        }

        return null;
    }
}
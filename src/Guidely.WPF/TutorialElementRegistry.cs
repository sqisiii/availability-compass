using System.Collections.Concurrent;
using System.Windows;

namespace Guidely.WPF;

/// <summary>
/// Registry tracking UI elements marked as tutorial targets.
/// Uses weak references to avoid memory leaks when elements are unloaded.
/// </summary>
public static class TutorialElementRegistry
{
    private static readonly ConcurrentDictionary<string, WeakReference<FrameworkElement>> Elements = new();

    /// <summary>
    /// Raised when an element is registered or unregistered.
    /// </summary>
    public static event EventHandler<string>? ElementChanged;

    /// <summary>
    /// Registers a framework element as a tutorial target.
    /// </summary>
    /// <param name="targetName">The unique name for this target.</param>
    /// <param name="element">The framework element.</param>
    public static void Register(string targetName, FrameworkElement element)
    {
        if (string.IsNullOrEmpty(targetName))
            return;

        Elements[targetName] = new WeakReference<FrameworkElement>(element);
        ElementChanged?.Invoke(null, targetName);
    }

    /// <summary>
    /// Unregisters a framework element from the registry.
    /// </summary>
    /// <param name="targetName">The name of the target to unregister.</param>
    public static void Unregister(string targetName)
    {
        if (string.IsNullOrEmpty(targetName))
            return;

        Elements.TryRemove(targetName, out _);
        ElementChanged?.Invoke(null, targetName);
    }

    /// <summary>
    /// Gets the element for a tutorial target, or null if not registered or garbage collected.
    /// </summary>
    /// <param name="targetName">The name of the target.</param>
    /// <returns>The framework element, or null if not found.</returns>
    public static FrameworkElement? GetElement(string? targetName)
    {
        if (string.IsNullOrEmpty(targetName))
            return null;

        if (Elements.TryGetValue(targetName, out var weakRef) &&
            weakRef.TryGetTarget(out var element))
        {
            return element;
        }

        return null;
    }

    /// <summary>
    /// Gets all registered element names.
    /// </summary>
    public static IEnumerable<string> GetAllTargetNames()
    {
        return Elements.Keys.ToList();
    }

    /// <summary>
    /// Clears all registered elements.
    /// </summary>
    public static void Clear()
    {
        Elements.Clear();
    }
}
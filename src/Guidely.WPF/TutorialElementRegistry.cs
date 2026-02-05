using System.Collections.Concurrent;
using System.Windows;

namespace Guidely.WPF;

/// <summary>
/// Registry tracking UI elements marked as tutorial targets.
/// Uses weak references to avoid memory leaks when elements are unloaded.
/// Multiple elements can share the same target name (e.g., buttons in a list).
/// </summary>
public static class TutorialElementRegistry
{
    private static readonly ConcurrentDictionary<string, List<WeakReference<FrameworkElement>>> Elements = new();
    private static readonly ReaderWriterLockSlim Lock = new(LockRecursionPolicy.NoRecursion);

    /// <summary>
    /// Raised when an element is registered or unregistered.
    /// </summary>
    public static event EventHandler<string>? ElementChanged;

    /// <summary>
    /// Registers a framework element as a tutorial target.
    /// Multiple elements can share the same target name.
    /// </summary>
    /// <param name="targetName">The name for this target.</param>
    /// <param name="element">The framework element.</param>
    public static void Register(string targetName, FrameworkElement element)
    {
        if (string.IsNullOrEmpty(targetName))
            return;

        Lock.EnterWriteLock();
        try
        {
            var list = Elements.GetOrAdd(targetName, _ => []);

            // Check if this element is already registered (avoid duplicates)
            var alreadyRegistered = list.Any(wr => wr.TryGetTarget(out var existing) && ReferenceEquals(existing, element));
            if (!alreadyRegistered)
            {
                list.Add(new WeakReference<FrameworkElement>(element));
            }
        }
        finally
        {
            Lock.ExitWriteLock();
        }

        ElementChanged?.Invoke(null, targetName);
    }

    /// <summary>
    /// Unregisters a specific framework element from the registry.
    /// </summary>
    /// <param name="targetName">The name of the target.</param>
    /// <param name="element">The specific element to unregister.</param>
    public static void Unregister(string targetName, FrameworkElement element)
    {
        if (string.IsNullOrEmpty(targetName))
        {
            return;
        }

        Lock.EnterWriteLock();
        try
        {
            if (!Elements.TryGetValue(targetName, out var list))
            {
                return;
            }

            // Remove the specific element and any dead weak references
            list.RemoveAll(wr => !wr.TryGetTarget(out var existing) || ReferenceEquals(existing, element));

            // Remove the entry if no elements remain
            if (list.Count == 0)
            {
                Elements.TryRemove(targetName, out _);
            }
        }
        finally
        {
            Lock.ExitWriteLock();
        }

        ElementChanged?.Invoke(null, targetName);
    }

    /// <summary>
    /// Unregisters all elements with the given target name.
    /// </summary>
    /// <param name="targetName">The name of the target to unregister.</param>
    public static void UnregisterAll(string targetName)
    {
        if (string.IsNullOrEmpty(targetName))
        {
            return;
        }

        Lock.EnterWriteLock();
        try
        {
            Elements.TryRemove(targetName, out _);
        }
        finally
        {
            Lock.ExitWriteLock();
        }

        ElementChanged?.Invoke(null, targetName);
    }

    /// <summary>
    /// Gets the first element for a tutorial target, or null if not registered or garbage collected.
    /// For backward compatibility - prefer GetElements() for multiple elements.
    /// </summary>
    /// <param name="targetName">The name of the target.</param>
    /// <returns>The first framework element, or null if not found.</returns>
    public static FrameworkElement? GetElement(string? targetName)
    {
        var elements = GetElements(targetName);
        return elements.FirstOrDefault();
    }

    /// <summary>
    /// Gets all elements for a tutorial target name.
    /// </summary>
    /// <param name="targetName">The name of the target.</param>
    /// <returns>List of all registered elements for this target name.</returns>
    public static IReadOnlyList<FrameworkElement> GetElements(string? targetName)
    {
        if (string.IsNullOrEmpty(targetName))
        {
            return [];
        }

        Lock.EnterReadLock();
        try
        {
            if (!Elements.TryGetValue(targetName, out var list))
            {
                return [];
            }

            var result = new List<FrameworkElement>();

            foreach (var weakRef in list)
            {
                if (weakRef.TryGetTarget(out var element))
                {
                    result.Add(element);
                }
            }

            return result;
        }
        finally
        {
            Lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Gets all registered element names.
    /// </summary>
    public static IEnumerable<string> GetAllTargetNames()
    {
        Lock.EnterReadLock();
        try
        {
            return Elements.Keys.ToList();
        }
        finally
        {
            Lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Clears all registered elements.
    /// </summary>
    public static void Clear()
    {
        Lock.EnterWriteLock();
        try
        {
            Elements.Clear();
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }
}
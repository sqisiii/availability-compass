using System.Windows;
using System.Windows.Media;

namespace AvailabilityCompass.WpfClient.Shared;

/// <summary>
/// Provides helper methods for working with XAML elements.
/// </summary>
public static class XamlHelper
{
    /// <summary>
    /// Finds all visual children of a specified type within a given dependency object.
    /// </summary>
    /// <typeparam name="T">The type of visual children to find.</typeparam>
    /// <param name="depObj">The dependency object to search within.</param>
    /// <returns>An enumerable collection of visual children of the specified type.</returns>
    public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject? depObj) where T : DependencyObject
    {
        if (depObj == null)
        {
            yield break;
        }

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);
            if (child is T t)
                yield return t;

            foreach (var childOfChild in FindVisualChildren<T>(child))
                yield return childOfChild;
        }
    }
}

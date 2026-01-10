using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AvailabilityCompass.Core.Shared;

/// <summary>
/// An ObservableCollection that supports batch operations to minimize UI updates.
/// Use ReplaceAll for sorting operations to avoid individual CollectionChanged events.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public sealed class RangeObservableCollection<T> : ObservableCollection<T>
{
    private bool _suppressNotification;

    /// <summary>
    /// Replaces all items in the collection with a single Reset notification.
    /// This is much faster than Clear() + AddRange() for large collections.
    /// </summary>
    /// <param name="items">The items to replace the collection with.</param>
    public void ReplaceAll(IEnumerable<T> items)
    {
        _suppressNotification = true;

        Items.Clear();
        foreach (var item in items)
        {
            Items.Add(item);
        }

        _suppressNotification = false;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Adds a range of items with a single Reset notification.
    /// </summary>
    /// <param name="items">The items to add.</param>
    public void AddRange(IEnumerable<T> items)
    {
        _suppressNotification = true;

        foreach (var item in items)
        {
            Items.Add(item);
        }

        _suppressNotification = false;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!_suppressNotification)
        {
            base.OnCollectionChanged(e);
        }
    }
}

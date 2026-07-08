using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AvailabilityCompass.Core.Shared;

/// <summary>
/// A collection that is fully observable, meaning it notifies listeners of changes to the items within the collection,
/// as well as changes to the collection itself.
/// </summary>
/// <typeparam name="T">The type of elements in the collection, which must implement <see cref="INotifyPropertyChanged"/>.</typeparam>
public sealed class FullyObservableCollection<T> : ObservableCollection<T>
    where T : INotifyPropertyChanged
{
    private readonly string[] _observedItemProperties;
    private readonly SynchronizationContext _syncContext;

    /// <param name="observedItemProperties">
    /// When provided, only changes to these item properties are escalated to collection-level
    /// Replace events. Without a filter every item property change (e.g. a periodic timer tick)
    /// forces consumers to rebuild views bound to the collection.
    /// </param>
    public FullyObservableCollection(params string[] observedItemProperties)
    {
        _observedItemProperties = observedItemProperties;
        _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
        CollectionChanged += FullObservableCollectionCollectionChanged;
    }

    public FullyObservableCollection(IEnumerable<T> items) : this()
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    protected override void ClearItems()
    {
        // Clear() raises Reset with no OldItems, so the handlers must be detached
        // here or every cleared item stays subscribed forever.
        foreach (var item in Items)
        {
            item.PropertyChanged -= ItemPropertyChanged;
        }

        base.ClearItems();
    }

    private void FullObservableCollectionCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
            }
        }

        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
            }
        }
    }

    private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender == null)
            return;

        // Null/empty property name means "everything changed" and is always escalated.
        if (_observedItemProperties.Length > 0
            && !string.IsNullOrEmpty(e.PropertyName)
            && !_observedItemProperties.Contains(e.PropertyName))
        {
            return;
        }

        var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
        if (args.NewStartingIndex == -1 || args.OldStartingIndex == -1)
            return;

        if (SynchronizationContext.Current == _syncContext)
        {
            OnCollectionChanged(args);
        }
        else
        {
            _syncContext.Post(_ => OnCollectionChanged(args), null);
        }
    }
}

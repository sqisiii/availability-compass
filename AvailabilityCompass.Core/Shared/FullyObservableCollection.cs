using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AvailabilityCompass.Core.Shared;

public sealed class FullyObservableCollection<T> : ObservableCollection<T>
    where T : INotifyPropertyChanged
{
    private readonly SynchronizationContext _syncContext;

    public FullyObservableCollection()
    {
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
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AvailabilityCompass.Core.Shared.EventBus;

/// <summary>
/// Reactive Extensions-based implementation of <see cref="IEventBus"/> for cross-slice event communication.
/// </summary>
public class EventBus : IEventBus
{
    // Synchronize: handlers publish from thread-pool threads, and concurrent OnNext
    // on a bare Subject violates the Rx serialization contract.
    private readonly ISubject<object> _subject = Subject.Synchronize(new Subject<object>());

    /// <inheritdoc />
    public void Publish<TEvent>(TEvent evt)
    {
        if (evt != null)
        {
            _subject.OnNext(evt);
        }
    }

    /// <inheritdoc />
    public IObservable<TEvent> Listen<TEvent>()
    {
        return _subject.OfType<TEvent>();
    }

    /// <inheritdoc />
    public IObservable<object> ListenToAll()
    {
        return _subject.AsObservable();
    }
}
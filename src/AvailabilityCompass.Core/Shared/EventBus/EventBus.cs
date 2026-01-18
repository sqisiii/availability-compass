using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AvailabilityCompass.Core.Shared.EventBus;

/// <summary>
/// Reactive Extensions-based implementation of <see cref="IEventBus"/> for cross-slice event communication.
/// </summary>
public class EventBus : IEventBus
{
    private readonly ISubject<object> _subject = new Subject<object>();

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
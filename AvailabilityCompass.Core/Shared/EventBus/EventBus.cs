using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AvailabilityCompass.Core.Shared.EventBus;

public class EventBus : IEventBus
{
    private readonly ISubject<object> _subject = new Subject<object>();

    public void Publish<TEvent>(TEvent evt)
    {
        if (evt != null)
        {
            _subject.OnNext(evt);
        }
    }

    public IObservable<TEvent> Listen<TEvent>()
    {
        return _subject.OfType<TEvent>();
    }

    public IObservable<object> ListenToAll()
    {
        return _subject.AsObservable();
    }
}
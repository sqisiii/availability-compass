namespace AvailabilityCompass.Core.Shared.EventBus;

public interface IEventBus
{
    void Publish<TEvent>(TEvent evt);
    IObservable<TEvent> Listen<TEvent>();
    IObservable<object> ListenToAll();
}
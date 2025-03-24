namespace AvailabilityCompass.Core.Shared.EventBus;

/// <summary>
/// Interface for an event bus that allows publishing and listening to events.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an event to the event bus.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="evt">The event to publish.</param>
    void Publish<TEvent>(TEvent evt);

    /// <summary>
    /// Listens for events of a specific type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to listen for.</typeparam>
    /// <returns>An observable sequence of events of the specified type.</returns>
    IObservable<TEvent> Listen<TEvent>();

    /// <summary>
    /// Listens for all events.
    /// </summary>
    /// <returns>An observable sequence of all events.</returns>
    IObservable<object> ListenToAll();
}

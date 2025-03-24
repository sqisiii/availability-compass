namespace AvailabilityCompass.Core.Shared;

/// <summary>
/// Defines a generic interface for an abstract factory that creates instances of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of object that the factory creates.</typeparam>
public interface IAbstractFactory<out T>
{
    /// <summary>
    /// Creates an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>An instance of type <typeparamref name="T"/>.</returns>
    T Create();
}

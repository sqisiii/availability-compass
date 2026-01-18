namespace AvailabilityCompass.Core.Shared;

/// <summary>
/// Generic implementation of <see cref="IAbstractFactory{T}"/> that uses a delegate for object creation.
/// </summary>
/// <typeparam name="T">The type of object that the factory creates.</typeparam>
public class AbstractFactory<T> : IAbstractFactory<T>
{
    private readonly Func<T> _factory;

    public AbstractFactory(Func<T> factory)
    {
        _factory = factory;
    }

    /// <inheritdoc />
    public T Create()
    {
        return _factory();
    }
}
namespace AvailabilityCompass.Core.Shared;

public interface IAbstractFactory<out T>
{
    T Create();
}
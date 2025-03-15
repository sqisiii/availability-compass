namespace AvailabilityCompass.Core.Application.Database;

public interface IDbInitializer
{
    Task InitializeAsync();
}
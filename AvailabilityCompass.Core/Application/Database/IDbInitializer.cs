namespace AvailabilityCompass.Core.Application.Database;

/// <summary>
/// Interface for database initialization operations.
/// </summary>
public interface IDbInitializer
{
    /// <summary>
    /// Initializes the database asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InitializeAsync();
}
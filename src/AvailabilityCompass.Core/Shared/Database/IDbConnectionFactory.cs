using System.Data;

namespace AvailabilityCompass.Core.Shared.Database;

/// <summary>
/// Represents a factory for creating database connections.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates and returns a new instance of a database connection.
    /// </summary>
    /// <returns>An instance of <see cref="IDbConnection"/>.</returns>
    IDbConnection Connect();
}

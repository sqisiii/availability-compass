using System.Data;
using AvailabilityCompass.Core.Application.Database;
using Microsoft.Data.Sqlite;

namespace AvailabilityCompass.Core.Shared.Database;

/// <summary>
/// Factory class for creating SQLite database connections.
/// </summary>
public class SqliteDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteDbConnectionFactory"/> class with the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to use for the SQLite database.</param>
    public SqliteDbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Creates and returns a new instance of an SQLite database connection.
    /// </summary>
    /// <returns>An instance of <see cref="IDbConnection"/> representing the SQLite database connection.</returns>
    public IDbConnection Connect()
    {
        SqliteDapperTypeHandlers.EnsureRegistered();
        return new SqliteConnection(_connectionString);
    }
}

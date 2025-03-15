using System.Data;
using System.Data.SQLite;

namespace AvailabilityCompass.Core.Shared.Database;

public class SqliteDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteDbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Connect()
    {
        return new SQLiteConnection(_connectionString);
    }
}
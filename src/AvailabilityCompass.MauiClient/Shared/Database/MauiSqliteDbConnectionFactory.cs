using System.Data;
using AvailabilityCompass.Core.Application.Database;
using AvailabilityCompass.Core.Shared.Database;
using Microsoft.Data.Sqlite;

namespace AvailabilityCompass.MauiClient.Shared.Database;

public class MauiSqliteDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MauiSqliteDbConnectionFactory(string connectionStringTemplate)
    {
        _connectionString = connectionStringTemplate.Replace(
            "{AppDataDirectory}",
            FileSystem.AppDataDirectory);
    }

    public IDbConnection Connect()
    {
        SqliteDapperTypeHandlers.EnsureRegistered();
        return new SqliteConnection(_connectionString);
    }
}

namespace AvailabilityCompass.Core.Application.Options;

/// <summary>
/// Configuration options for SQLite database connection.
/// </summary>
public class SqliteDbOptions
{
    public string ConnectionString { get; init; } = string.Empty;
}
using System.Data;
using AvailabilityCompass.Core.Shared.Database;
using Dapper;

namespace AvailabilityCompass.Core.Application.Database;

/// <summary>
/// SQLite database initializer that creates required tables and registers custom type handlers.
/// </summary>
public class SqlDbInitializer : IDbInitializer
{
    private readonly IDbConnectionFactory _sqliteDbConnectionFactory;

    public SqlDbInitializer(IDbConnectionFactory sqliteDbConnectionFactory)
    {
        _sqliteDbConnectionFactory = sqliteDbConnectionFactory;
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        SqliteDapperTypeHandlers.EnsureRegistered();

        await EnableWriteAheadLoggingAsync();
        await PrepareSourceTablesAsync();
        await PrepareCalendarTablesAsync();
        await NormalizeGuidStorageAsync();
        await PrepareSettingsTableAsync();
        await PrepareDisabledSourcesTableAsync();
    }

    private async Task EnableWriteAheadLoggingAsync()
    {
        // WAL is persistent per database file; readers no longer block on writers
        // during the bulk source-refresh transactions.
        using var database = _sqliteDbConnectionFactory.Connect();
        await database.ExecuteAsync("PRAGMA journal_mode=WAL;");
    }

    private async Task PrepareSourceTablesAsync()
    {
        const string createSourceTable =
            """
            CREATE TABLE IF NOT EXISTS Source (
                            SourceId TEXT NOT NULL,
                            SeqNo INTEGER,
                            Title TEXT NOT NULL,
                            Url TEXT NOT NULL,
                            StartDate TEXT NOT NULL,
                            EndDate TEXT,
                            ChangeDate TEXT NOT NULL,
                            PRIMARY KEY (SourceId, SeqNo)
                        );
            """;
        // language=SQLite
        const string createSourceAdditionalDataTables =
            """
            CREATE TABLE IF NOT EXISTS SourceAdditionalData (
                            SourceId TEXT NOT NULL,
                            SourceSeqNo INTEGER NOT NULL,
                            Key TEXT NOT NULL,
                            Value TEXT,
                            PRIMARY KEY (SourceId, SourceSeqNo, Key),
                            FOREIGN KEY (SourceId, SourceSeqNo) REFERENCES Source(SourceId, SeqNo) ON DELETE CASCADE
                        );
            """;
        using var database = _sqliteDbConnectionFactory.Connect();
        await database.ExecuteAsync(createSourceTable);
        await database.ExecuteAsync(createSourceAdditionalDataTables);
    }

    private async Task PrepareCalendarTablesAsync()
    {
        // language=SQLite
        const string createCalendarTable =
            """
            CREATE TABLE IF NOT EXISTS Calendar (
                            CalendarId BLOB(16) NOT NULL,
                            Name TEXT NOT NULL,
                            IsOnly Integer NOT NULL,
                            ChangeDate TEXT NOT NULL,
                            PRIMARY KEY (CalendarId)
                        );
            """;
        // language=SQLite
        const string createDateEntryTable =
            """
            CREATE TABLE IF NOT EXISTS DateEntry (
                            CalendarId BLOB(16) NOT NULL,
                            Id BLOB(16) NOT NULL,
                            StartDate TEXT NOT NULL,
                            Description TEXT,
                            IsRecurring INTEGER NOT NULL DEFAULT 0,
                            Duration INTEGER NOT NULL DEFAULT 1,
                            Frequency INTEGER,
                            NumberOfRepetitions INTEGER NOT NULL DEFAULT 0,
                            ChangeDate TEXT NOT NULL,
                            PRIMARY KEY (CalendarId, Id),
                            FOREIGN KEY (CalendarId) REFERENCES Calendar(CalendarId) ON DELETE CASCADE
                        );
            """;

        using var database = _sqliteDbConnectionFactory.Connect();
        await database.ExecuteAsync(createCalendarTable);
        await database.ExecuteAsync(createDateEntryTable);
    }

    private async Task NormalizeGuidStorageAsync()
    {
        using var database = _sqliteDbConnectionFactory.Connect();
        database.Open();
        await database.ExecuteAsync("PRAGMA foreign_keys=OFF;");

        using var transaction = database.BeginTransaction();
        await NormalizeGuidColumnAsync(database, transaction, "Calendar", "CalendarId");
        await NormalizeGuidColumnAsync(database, transaction, "DateEntry", "CalendarId");
        await NormalizeGuidColumnAsync(database, transaction, "DateEntry", "Id");
        transaction.Commit();

        await database.ExecuteAsync("PRAGMA foreign_keys=ON;");
    }

    private static async Task NormalizeGuidColumnAsync(
        IDbConnection database,
        IDbTransaction transaction,
        string tableName,
        string columnName)
    {
        string selectSql = $"""
                            SELECT rowid AS RowId, {columnName} AS Value
                            FROM {tableName}
                            WHERE typeof({columnName}) = 'text';
                            """;

        var legacyValues = await database.QueryAsync<LegacyGuidValue>(selectSql, transaction: transaction)
            .ConfigureAwait(false);

        foreach (LegacyGuidValue legacyValue in legacyValues)
        {
            if (!Guid.TryParse(legacyValue.Value, out Guid guid))
            {
                throw new DataException(
                    $"Invalid GUID value in {tableName}.{columnName} at rowid {legacyValue.RowId}.");
            }

            string updateSql = $"""
                                UPDATE {tableName}
                                SET {columnName} = @Value
                                WHERE rowid = @RowId;
                                """;

            await database.ExecuteAsync(
                    updateSql,
                    new { Value = guid.ToByteArray(), legacyValue.RowId },
                    transaction)
                .ConfigureAwait(false);
        }
    }

    private async Task PrepareSettingsTableAsync()
    {
        // language=SQLite
        const string createSettingsTable =
            """
            CREATE TABLE IF NOT EXISTS Setting (
                            Key TEXT NOT NULL PRIMARY KEY,
                            Value TEXT NOT NULL,
                            ChangeDate TEXT NOT NULL
                        );
            """;
        using var database = _sqliteDbConnectionFactory.Connect();
        await database.ExecuteAsync(createSettingsTable);
    }

    private async Task PrepareDisabledSourcesTableAsync()
    {
        // language=SQLite
        const string createDisabledSourcesTable =
            """
            CREATE TABLE IF NOT EXISTS DisabledSources (
                            SourceId TEXT NOT NULL PRIMARY KEY,
                            ChangeDate TEXT NOT NULL
                        );
            """;
        using var database = _sqliteDbConnectionFactory.Connect();
        await database.ExecuteAsync(createDisabledSourcesTable);
    }

    private sealed class LegacyGuidValue
    {
        public long RowId { get; init; }
        public string Value { get; init; } = string.Empty;
    }
}

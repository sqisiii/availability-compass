using AvailabilityCompass.Core.Shared.Database;
using Dapper;

namespace AvailabilityCompass.Core.Application.Database;

public class SqlDbInitializer : IDbInitializer
{
    private readonly IDbConnectionFactory _sqliteDbConnectionFactory;

    public SqlDbInitializer(IDbConnectionFactory sqliteDbConnectionFactory)
    {
        _sqliteDbConnectionFactory = sqliteDbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        //Sqlite doesn't have a DateTime type, so we need to register a custom type handler for DateOnly
        SqlMapper.AddTypeHandler(new SqliteDateOnlyTypeHandler());
        //Guid is stored as Blob(16) so a custom type handler is needed
        SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());
        await PrepareSourceTablesAsync();
        await PrepareCalendarTablesAsync();
        await PrepareSettingsTableAsync();
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
}
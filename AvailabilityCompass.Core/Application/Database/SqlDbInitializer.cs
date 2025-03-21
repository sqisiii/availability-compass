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
        SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());
        await PrepareSourceTablesAsync();
        await PrepareCalendarTablesAsync();
    }

    private async Task PrepareSourceTablesAsync()
    {
        const string createSourceTable =
            @"CREATE TABLE IF NOT EXISTS Source (
                SourceId TEXT NOT NULL,
                SeqNo INTEGER,
                Title TEXT NOT NULL,
                Url TEXT NOT NULL,
                StartDate TEXT NOT NULL,
                EndDate TEXT,
                ChangeDate TEXT NOT NULL,
                PRIMARY KEY (SourceId, SeqNo)
            );";
        const string createSourceAdditionalDataTables =
            @"CREATE TABLE IF NOT EXISTS SourceAdditionalData (
                SourceId TEXT NOT NULL,
                SourceSeqNo INTEGER NOT NULL,
                Key TEXT NOT NULL,
                Value TEXT,
                PRIMARY KEY (SourceId, SourceSeqNo, Key),
                FOREIGN KEY (SourceId, SourceSeqNo) REFERENCES Source(SourceId, SeqNo) ON DELETE CASCADE
            );";
        using var database = _sqliteDbConnectionFactory.Connect();
        await database.ExecuteAsync(createSourceTable);
        await database.ExecuteAsync(createSourceAdditionalDataTables);
    }

    private async Task PrepareCalendarTablesAsync()
    {
        const string createCalendarTable =
            @"CREATE TABLE IF NOT EXISTS Calendar (
                CalendarId BLOB(16) NOT NULL,
                Name TEXT NOT NULL,
                IsOnly Integer NOT NULL,
                ChangeDate TEXT NOT NULL,
                PRIMARY KEY (CalendarId)
            );";
        const string createSingleDateTable =
            @"CREATE TABLE IF NOT EXISTS SingleDate (
                CalendarId BLOB(16) NOT NULL,
                Id BLOB(16) NOT NULL,
                Date TEXT NOT NULL,
                Description TEXT,
                PRIMARY KEY (CalendarId, Id),
                FOREIGN KEY (CalendarId) REFERENCES Calendar(CalendarId) ON DELETE CASCADE
            );";
        const string createRecurringDateTable =
            @"CREATE TABLE IF NOT EXISTS RecurringDate (
                CalendarId BLOB(16) NOT NULL,
                Id BLOB(16) NOT NULL,
                StartDate TEXT NOT NULL,
                DaysCount INTEGER NOT NULL,
                Description TEXT,
                PRIMARY KEY (CalendarId, Id),
                FOREIGN KEY (CalendarId) REFERENCES Calendar(CalendarId) ON DELETE CASCADE
            );";
        using var database = _sqliteDbConnectionFactory.Connect();
        await database.ExecuteAsync(createCalendarTable);
        await database.ExecuteAsync(createSingleDateTable);
        await database.ExecuteAsync(createRecurringDateTable);
    }
}
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
        await PrepareSourceTablesAsync();
    }

    private async Task PrepareSourceTablesAsync()
    {
        const string createSourceTables =
            @"CREATE TABLE IF NOT EXISTS Source (
                SourceId TEXT NOT NULL,
                SeqNo INTEGER,
                Title TEXT NOT NULL,
                Type TEXT NOT NULL,
                Country TEXT NOT NULL,
                StartDate TEXT NOT NULL,
                EndDate TEXT,
                Price REAL NOT NULL,
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
                FOREIGN KEY (SourceId, SourceSeqNo) REFERENCES Source(SourceId, Id) ON DELETE CASCADE
            );";
        using var database = _sqliteDbConnectionFactory.Connect();
        await database.ExecuteAsync(createSourceTables);
        await database.ExecuteAsync(createSourceAdditionalDataTables);
    }
}
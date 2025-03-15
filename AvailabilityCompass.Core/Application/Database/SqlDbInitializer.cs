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
                IntegrationId TEXT NOT NULL,
                SeqNo INTEGER,
                Title TEXT NOT NULL,
                Type TEXT NOT NULL,
                Country TEXT NOT NULL,
                StartDate TEXT NOT NULL,
                EndDate TEXT,
                Price REAL NOT NULL,
                ChangeDate TEXT NOT NULL,
                PRIMARY KEY (IntegrationId, SeqNo)
            );";
        const string createSourceAdditionalDatatables =
            @"CREATE TABLE IF NOT EXISTS SourceAdditionalData (
                IntegrationId TEXT NOT NULL,
                SourceSeqNo INTEGER NOT NULL,
                Key TEXT NOT NULL,
                Value TEXT,
                PRIMARY KEY (IntegrationId, SourceSeqNo, Key),
                FOREIGN KEY (IntegrationId, SourceSeqNo) REFERENCES Source(IntegrationId, Id) ON DELETE CASCADE
            );";
        using var database = _sqliteDbConnectionFactory.Connect();
        await database.ExecuteAsync(createSourceTables);
        await database.ExecuteAsync(createSourceAdditionalDatatables);
    }
}
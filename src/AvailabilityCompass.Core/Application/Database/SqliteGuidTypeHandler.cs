using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace AvailabilityCompass.Core.Application.Database;

/// <summary>
/// Type handler for SQLite to support storing and retrieving GUIDs as binary data.
/// This class enables Dapper to correctly map between .NET's Guid type and SQLite's BLOB storage.
/// </summary>
internal class SqliteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        // Force Microsoft.Data.Sqlite to bind as BLOB instead of default Guid->TEXT mapping.
        if (parameter is SqliteParameter sqliteParameter)
        {
            sqliteParameter.SqliteType = SqliteType.Blob;
        }

        parameter.Value = value.ToByteArray();
        parameter.DbType = DbType.Binary;
    }

    public override Guid Parse(object value)
    {
        return value switch
        {
            // Preferred storage format: 16-byte BLOB.
            byte[] bytes when bytes.Length == 16 => new Guid(bytes),
            // Backward compatibility for rows stored as TEXT GUID.
            string text when Guid.TryParse(text, out var guid) => guid,
            _ => throw new DataException("Invalid GUID data retrieved from database. Expected BLOB(16) or GUID string.")
        };
    }
}

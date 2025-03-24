using System.Data;
using Dapper;

namespace AvailabilityCompass.Core.Application.Database;

/// <summary>
/// Type handler for SQLite to support storing and retrieving GUIDs as binary data.
/// This class enables Dapper to correctly map between .NET's Guid type and SQLite's BLOB storage.
/// </summary>
internal class SqliteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        // Store the GUID as a 16-byte BLOB (binary representation)
        parameter.Value = value.ToByteArray();
        parameter.DbType = DbType.Binary;
    }

    public override Guid Parse(object value)
    {
        // SQLite returns a byte[] when reading BLOB
        if (value is byte[] bytes && bytes.Length == 16)
        {
            return new Guid(bytes);
        }

        throw new DataException("Invalid GUID blob data retrieved from database.");
    }
}
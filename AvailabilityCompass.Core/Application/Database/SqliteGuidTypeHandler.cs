using System.Data;
using Dapper;

namespace AvailabilityCompass.Core.Application.Database;

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
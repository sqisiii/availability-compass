using System.Data;
using Dapper;

namespace AvailabilityCompass.Core.Application.Database;

/// <summary>
/// Handles the conversion between DateOnly objects and SQLite string values.
/// This type handler enables DateOnly type support in Dapper for SQLite databases.
/// </summary>
public class SqliteDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.Value = value.ToString("yyyy-MM-dd");
        parameter.DbType = DbType.String;
    }

    public override DateOnly Parse(object value)
    {
        return DateOnly.Parse((string)value);
    }
}
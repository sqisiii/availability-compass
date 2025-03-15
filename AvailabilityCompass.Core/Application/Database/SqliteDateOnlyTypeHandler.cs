using System.Data;
using Dapper;

namespace AvailabilityCompass.Core.Application.Database;

public class SqliteDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.Value = value.ToString("yyyy-MM-dd"); // Store as text
        parameter.DbType = DbType.String;
    }

    public override DateOnly Parse(object value)
    {
        return DateOnly.Parse((string)value); // Convert back to DateOnly
    }
}
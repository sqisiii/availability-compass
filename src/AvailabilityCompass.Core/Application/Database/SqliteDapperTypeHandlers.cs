using Dapper;

namespace AvailabilityCompass.Core.Application.Database;

/// <summary>
/// Registers SQLite-specific Dapper handlers before any query mapper can be cached.
/// </summary>
public static class SqliteDapperTypeHandlers
{
    private static readonly Lock SyncRoot = new();
    private static volatile bool _registered;

    public static void EnsureRegistered()
    {
        // Double-checked locking: this runs on every connection open, so the
        // volatile fast-path read avoids taking the lock once registration is done.
        // ReSharper disable once InconsistentlySynchronizedField
        if (_registered)
        {
            return;
        }

        lock (SyncRoot)
        {
            if (_registered)
            {
                return;
            }

            SqlMapper.AddTypeHandler(new SqliteDateOnlyTypeHandler());

            // Guid is stored as BLOB(16), so override Dapper's built-in Guid type map.
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
            SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());

            // If any page raced initialization and cached a mapper, discard it now.
            SqlMapper.PurgeQueryCache();
            _registered = true;
        }
    }
}

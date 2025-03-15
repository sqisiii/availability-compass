using System.Data;

namespace AvailabilityCompass.Core.Shared.Database;

public interface IDbConnectionFactory
{
    IDbConnection Connect();
}
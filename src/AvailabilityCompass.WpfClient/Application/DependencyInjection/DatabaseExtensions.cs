using AvailabilityCompass.Core.Application.Database;
using AvailabilityCompass.Core.Application.Options;
using AvailabilityCompass.Core.Shared.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SqliteDbOptions>(configuration.GetSection("SqliteDbOptions"));
        services.AddSingleton<IDbConnectionFactory, SqliteDbConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SqliteDbOptions>>().Value;
            return new SqliteDbConnectionFactory(options.ConnectionString);
        });
        services.AddSingleton<IDbInitializer, SqlDbInitializer>();

        return services;
    }
}
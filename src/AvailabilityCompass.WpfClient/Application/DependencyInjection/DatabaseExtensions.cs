using AvailabilityCompass.Core.Application.Database;
using AvailabilityCompass.Core.Application.Options;
using AvailabilityCompass.Core.Shared.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

/// <summary>
/// Extension methods for registering database services in the DI container.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Adds database services including SQLite connection factory and initializer.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
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
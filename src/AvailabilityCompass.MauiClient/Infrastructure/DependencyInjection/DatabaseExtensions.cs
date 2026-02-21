using AvailabilityCompass.Core.Application.Database;
using AvailabilityCompass.Core.Application.Options;
using AvailabilityCompass.Core.Shared.Database;
using AvailabilityCompass.MauiClient.Shared.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AvailabilityCompass.MauiClient.Infrastructure.DependencyInjection;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SqliteDbOptions>(configuration.GetSection("SqliteDbOptions"));
        services.AddSingleton<IDbConnectionFactory, MauiSqliteDbConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SqliteDbOptions>>().Value;
            return new MauiSqliteDbConnectionFactory(options.ConnectionString);
        });
        services.AddSingleton<IDbInitializer, SqlDbInitializer>();

        return services;
    }
}
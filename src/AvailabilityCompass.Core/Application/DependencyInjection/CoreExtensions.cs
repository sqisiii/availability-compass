using AvailabilityCompass.Core.Shared.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.Core.Application.DependencyInjection;

/// <summary>
/// Extension methods for registering core services with the dependency injection container.
/// </summary>
public static class CoreExtensions
{
    /// <summary>
    /// Registers Core services.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CoreExtensions).Assembly));
        services.AddHttpClient();
        services.AddSingleton<IEventBus, EventBus>();

        return services;
    }
}
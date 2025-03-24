using AvailabilityCompass.Core.Shared.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.Core.Application.DependencyInjection;

public static class CoreExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CoreExtensions).Assembly));
        services.AddHttpClient();
        services.AddSingleton<IEventBus, EventBus>();

        return services;
    }
}
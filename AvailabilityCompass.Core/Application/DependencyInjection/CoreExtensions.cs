using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.Core.Application.DependencyInjection;

public static class CoreExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CoreExtensions).Assembly));
        services.AddHttpClient();

        return services;
    }
}
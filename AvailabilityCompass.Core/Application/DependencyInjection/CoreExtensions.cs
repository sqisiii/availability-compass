using System.Reflection;
using AvailabilityCompass.Core.Features.ManageSources.Sources;
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

        var sourceServiceTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(ISourceService).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in sourceServiceTypes)
        {
            var attr = type.GetCustomAttribute<SourceServiceAttribute>();
            if (attr != null)
            {
                services.AddKeyedSingleton(typeof(ISourceService), attr.Key, (sp, _) => ActivatorUtilities.CreateInstance(sp, type));
            }
        }

        return services;
    }
}
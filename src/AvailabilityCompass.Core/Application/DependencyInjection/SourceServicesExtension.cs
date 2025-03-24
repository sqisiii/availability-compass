using System.Reflection;
using AvailabilityCompass.Core.Features.ManageSources.Sources;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.Core.Application.DependencyInjection;

/// <summary>
/// Extension methods for registering source services with the dependency injection container.
/// </summary>
public static class SourceServicesExtension
{
    public static IServiceCollection AddSourceServices(this IServiceCollection services)
    {
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
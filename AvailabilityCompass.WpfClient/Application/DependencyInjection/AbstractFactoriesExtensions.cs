using AvailabilityCompass.Core.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class AbstractFactoriesExtensions
{
    public static IServiceCollection AddAbstractFactory<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.AddTransient<TInterface, TImplementation>();
        services.AddSingleton<Func<TInterface>>(x => () => x.GetService<TInterface>()!);
        services.AddSingleton<IAbstractFactory<TInterface>, AbstractFactory<TInterface>>();

        return services;
    }

    public static IServiceCollection AddAbstractFactory<TImplementation>(this IServiceCollection services)
        where TImplementation : class
    {
        services.AddTransient<TImplementation>();
        services.AddSingleton<Func<TImplementation>>(x => () => x.GetService<TImplementation>()!);
        services.AddSingleton<IAbstractFactory<TImplementation>, AbstractFactory<TImplementation>>();

        return services;
    }
}
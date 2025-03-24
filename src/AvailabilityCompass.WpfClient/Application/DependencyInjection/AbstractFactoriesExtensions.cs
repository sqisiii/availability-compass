using AvailabilityCompass.Core.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

/// <summary>
/// Extension methods for adding abstract factories to the IServiceCollection.
/// </summary>
public static class AbstractFactoriesExtensions
{
    /// <summary>
    /// Adds an abstract factory for the specified interface and implementation to the IServiceCollection.
    /// </summary>
    /// <typeparam name="TInterface">The interface type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The IServiceCollection to add the factory to.</param>
    /// <returns>The IServiceCollection with the added factory.</returns>
    public static IServiceCollection AddAbstractFactory<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.AddTransient<TInterface, TImplementation>();
        services.AddSingleton<Func<TInterface>>(x => () => x.GetService<TInterface>()!);
        services.AddSingleton<IAbstractFactory<TInterface>, AbstractFactory<TInterface>>();

        return services;
    }

    /// <summary>
    /// Adds an abstract factory for the specified implementation to the IServiceCollection.
    /// </summary>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The IServiceCollection to add the factory to.</param>
    /// <returns>The IServiceCollection with the added factory.</returns>
    public static IServiceCollection AddAbstractFactory<TImplementation>(this IServiceCollection services)
        where TImplementation : class
    {
        services.AddTransient<TImplementation>();
        services.AddSingleton<Func<TImplementation>>(x => () => x.GetService<TImplementation>()!);
        services.AddSingleton<IAbstractFactory<TImplementation>, AbstractFactory<TImplementation>>();

        return services;
    }
}

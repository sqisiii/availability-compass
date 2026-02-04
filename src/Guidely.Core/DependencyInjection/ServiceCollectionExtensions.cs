using Guidely.Core.Abstractions;
using Guidely.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Guidely.Core.DependencyInjection;

/// <summary>
/// Extension methods for registering Guidely services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Guidely tutorial services to the service collection.
    /// </summary>
    /// <typeparam name="TContext">The tutorial context type.</typeparam>
    /// <typeparam name="TTrigger">The trigger enum type.</typeparam>
    /// <typeparam name="TGroup">The group enum type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure the tutorial.</param>
    /// <returns>The service collection for chaining.</returns>
    // ReSharper disable once ConvertToExtensionBlock
    public static IServiceCollection AddGuidely<TContext, TTrigger, TGroup>(
        this IServiceCollection services,
        Action<TutorialConfigBuilder<TContext, TTrigger, TGroup>> configure)
        where TContext : TutorialContextBase, new()
        where TTrigger : Enum
        where TGroup : Enum
    {
        var builder = new TutorialConfigBuilder<TContext, TTrigger, TGroup>();
        configure(builder);

        // Register the configuration as a factory that builds with the service provider
        services.AddSingleton(sp => builder.Build(sp));

        // Register the tutorial service
        services.AddSingleton<ITutorialService<TContext, TTrigger, TGroup>>(sp =>
        {
            var config = sp.GetRequiredService<TutorialConfiguration<TContext, TTrigger, TGroup>>();
            var persistence = sp.GetService<ITutorialPersistence>();
            return new TutorialService<TContext, TTrigger, TGroup>(config, persistence);
        });

        // Register the view model
        services.AddSingleton<TutorialViewModel<TContext, TTrigger, TGroup>>();

        return services;
    }

    /// <summary>
    /// Adds a custom persistence implementation for the tutorial.
    /// </summary>
    /// <typeparam name="TPersistence">The persistence implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGuidelyPersistence<TPersistence>(this IServiceCollection services)
        where TPersistence : class, ITutorialPersistence
    {
        services.AddSingleton<ITutorialPersistence, TPersistence>();
        return services;
    }

    /// <summary>
    /// Adds a custom persistence implementation for the tutorial using a factory.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">Factory function to create the persistence instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGuidelyPersistence(
        this IServiceCollection services,
        Func<IServiceProvider, ITutorialPersistence> factory)
    {
        services.AddSingleton(factory);
        return services;
    }
}
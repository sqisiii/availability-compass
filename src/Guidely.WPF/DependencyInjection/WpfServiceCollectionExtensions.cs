using Microsoft.Extensions.DependencyInjection;

namespace Guidely.WPF.DependencyInjection;

/// <summary>
/// Extension methods for registering Guidely WPF services.
/// </summary>
public static class WpfServiceCollectionExtensions
{
    /// <summary>
    /// Adds Guidely WPF services to the service collection.
    /// Call this after AddGuidely from Guidely.Core.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGuidelyWpf(this IServiceCollection services)
    {
        // Currently no additional WPF-specific services are required.
        // The TutorialOverlay control works directly with the ViewModel from Guidely.Core.
        // This method is provided for future extensibility and consistency.
        return services;
    }
}
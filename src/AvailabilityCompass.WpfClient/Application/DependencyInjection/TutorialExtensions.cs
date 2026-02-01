using AvailabilityCompass.Core.Features.Tutorial;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

/// <summary>
/// Extension methods for registering tutorial-related services in the DI container.
/// </summary>
public static class TutorialExtensions
{
    /// <summary>
    /// Adds tutorial feature services including the tutorial service and view model.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTutorial(this IServiceCollection services)
    {
        services.AddSingleton<ITutorialService, TutorialService>();
        services.AddSingleton<TutorialViewModel>();
        return services;
    }
}

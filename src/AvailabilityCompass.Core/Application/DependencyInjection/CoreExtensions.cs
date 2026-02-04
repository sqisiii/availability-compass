using AvailabilityCompass.Core.Features.Tutorial;
using AvailabilityCompass.Core.Features.Tutorial.Steps;
using AvailabilityCompass.Core.Shared.EventBus;
using Guidely.Core.DependencyInjection;
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

        // Register Guidely tutorial services
        services.AddGuidelyPersistence<MediatRTutorialPersistence>();
        services.AddGuidely<AvailabilityCompassContext, AppTutorialTrigger, AppTutorialGroup>(builder =>
        {
            builder.ScanStepsFromAssembly(typeof(WelcomeStep).Assembly);
            builder.ConfigureTransitions(TutorialSetup.ConfigureTransitions);
            builder.SetStartStep(TutorialStepIds.Welcome);
            builder.EnableAutoStart();
        });

        return services;
    }
}

using AvailabilityCompass.WpfClient.Application.Initialization;
using AvailabilityCompass.WpfClient.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DepedencyInjection;

public static class AddMainWindowExtensions
{
    public static IServiceCollection AddMainWindow(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<Bootstrapper>();
        return services;
    }
}
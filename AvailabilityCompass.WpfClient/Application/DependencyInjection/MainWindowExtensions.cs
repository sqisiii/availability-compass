using AvailabilityCompass.WpfClient.Application.Initialization;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class AddMainWindowExtensions
{
    public static IServiceCollection AddMainWindow(this IServiceCollection services)
    {
        services.AddSingleton<Bootstrapper>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
        return services;
    }
}
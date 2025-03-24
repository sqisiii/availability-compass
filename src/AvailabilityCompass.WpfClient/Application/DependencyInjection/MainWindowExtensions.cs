using AvailabilityCompass.WpfClient.Application.Initialization;
using AvailabilityCompass.WpfClient.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class MainWindowExtensions
{
    public static IServiceCollection AddMainWindow(this IServiceCollection services)
    {
        services.AddSingleton<Bootstrapper>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
        return services;
    }
}
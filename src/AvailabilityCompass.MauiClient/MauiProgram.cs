using System.Reflection;
using AvailabilityCompass.Core.Application.DependencyInjection;
using AvailabilityCompass.MauiClient.Infrastructure.DependencyInjection;
using AvailabilityCompass.MauiClient.Infrastructure.Initialization;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AvailabilityCompass.MauiClient;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // Load embedded appsettings.json
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("AvailabilityCompass.MauiClient.appsettings.json");
        var configuration = new ConfigurationBuilder()
            .AddJsonStream(stream!)
            .Build();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Configure Serilog
        var logPath = Path.Combine(FileSystem.AppDataDirectory, "Logs", "MauiApp.log");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(logPath,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Services.AddSingleton(Log.Logger);

        // Register Core services (includes MediatR, HttpClient, EventBus, Guidely)
        builder.Services.AddCore();
        builder.Services.AddSourceServices();

        // Register MAUI-specific services
        builder.Services.AddDatabase(configuration);
        builder.Services.AddNavigation();
        builder.Services.AddSettings();
        builder.Services.AddCalendar();
        builder.Services.AddSearch();
        builder.Services.AddSource();

        // Register bootstrapper
        builder.Services.AddSingleton<MauiBootstrapper>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
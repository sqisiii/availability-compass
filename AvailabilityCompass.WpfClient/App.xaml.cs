using System.IO;
using System.Windows;
using AvailabilityCompass.Core.Application.DependencyInjection;
using AvailabilityCompass.WpfClient.Application.DependencyInjection;
using AvailabilityCompass.WpfClient.Application.Initialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Settings.Configuration;

namespace AvailabilityCompass.WpfClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly IHost _host;

    public App()
    {
        var serilogOptions = new ConfigurationReaderOptions(typeof(FileLoggerConfigurationExtensions).Assembly, typeof(MapLoggerConfigurationExtensions).Assembly);
        _host = Host.CreateDefaultBuilder()
            .UseDefaultServiceProvider((_, options) => { options.ValidateScopes = true; })
            .ConfigureServices((context, services) =>
            {
                services.AddCore();
                services.AddCalendar();
                services.AddMainWindow();
                services.AddNavigation();
                services.AddDatabase(context.Configuration);
                services.AddSearch();
                services.AddSettings();
                services.AddSource();
                services.AddSingleton(
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .Enrich.WithThreadId()
                        .ReadFrom.Configuration(
                            new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .Build(), serilogOptions)
                        .WriteTo.Map("LogName", "WpfApp",
                            (logName, wt) => wt.File($"./Logs/{logName}.log",
                                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}:{ThreadId}] {Message} {NewLine}{Exception}"),
                            20)
                        .CreateLogger());
            })
            .Build();
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        //can't be done in any other way (without using GetRequiredService anty-pattern)
        var bootstrapper = _host.Services.GetRequiredService<Bootstrapper>();
        bootstrapper.Run();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            Log.Information("Application closes.");
            await Log.CloseAndFlushAsync();
            _host.Dispose();
            base.OnExit(e);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Threading;
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
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        var customCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
        customCulture.DateTimeFormat.DateSeparator = "-";

        CultureInfo.DefaultThreadCurrentCulture = customCulture;
        CultureInfo.DefaultThreadCurrentUICulture = customCulture;

        var serilogOptions = new ConfigurationReaderOptions(typeof(FileLoggerConfigurationExtensions).Assembly,
            typeof(MapLoggerConfigurationExtensions).Assembly);
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, config) => { config.AddJsonStream(GetEmbeddedAppSettingsStream()); })
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
                services.AddSourceServices();
                services.AddSingleton(
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .Enrich.WithThreadId()
                        .ReadFrom.Configuration(context.Configuration, serilogOptions)
                        .WriteTo.Map("LogName", "WpfApp",
                            (logName, wt) => wt.File($"./Logs/{logName}.log",
                                outputTemplate:
                                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}:{ThreadId}] {Message} {NewLine}{Exception}"),
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

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogAndShowError(e.Exception);
        e.Handled = true;
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
            LogAndShowError(ex);
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogAndShowError(e.Exception);
        e.SetObserved();
    }

    private static void LogAndShowError(Exception ex)
    {
        Log.Fatal(ex, "Unhandled exception");
        MessageBox.Show(ex.ToString(), "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private static Stream GetEmbeddedAppSettingsStream()
    {
        var resourceStream = typeof(App).Assembly.GetManifestResourceStream("appsettings.json");
        if (resourceStream is null)
        {
            throw new InvalidOperationException("Embedded appsettings.json resource not found.");
        }

        var memoryStream = new MemoryStream();
        resourceStream.CopyTo(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }
}
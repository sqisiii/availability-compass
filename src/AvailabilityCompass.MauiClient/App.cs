using AvailabilityCompass.MauiClient.Infrastructure.Initialization;
using AvailabilityCompass.MauiClient.Pages;
using Serilog;

namespace AvailabilityCompass.MauiClient;

public class App : Application
{
    private readonly AppViewModel _appViewModel;
    private readonly MauiBootstrapper _bootstrapper;
    private bool _initialized;

    public App(MauiBootstrapper bootstrapper, AppViewModel appViewModel)
    {
        _bootstrapper = bootstrapper;
        _appViewModel = appViewModel;

        // Rx pipelines and fire-and-forget loads swallow exceptions into unobserved
        // tasks; without this hook those failures leave no trace in the logs.
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Log.Error(e.Exception, "Unobserved task exception");
            e.SetObserved();
        };

        LoadResources();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = new AppShell();
        var window = new Window(shell);

        shell.Loaded += async (_, _) =>
        {
            // Loaded fires again when the platform view reattaches (e.g. Android
            // activity recreation); initialization must run only once.
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            try
            {
                await _bootstrapper.RunAsync();
                await _appViewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed during app initialization");
            }
        };

        return window;
    }

    private void LoadResources()
    {
        var primary = Color.FromArgb("#512BD4");
        var primaryDark = Color.FromArgb("#ac99ea");

        Resources["Primary"] = primary;
        Resources["PrimaryDark"] = primaryDark;
        Resources["Secondary"] = Color.FromArgb("#DFD8F7");
        Resources["Tertiary"] = Color.FromArgb("#2B0B98");
        Resources["White"] = Colors.White;
        Resources["Black"] = Colors.Black;
        Resources["Gray100"] = Color.FromArgb("#E1E1E1");
        Resources["Gray200"] = Color.FromArgb("#C8C8C8");
        Resources["Gray300"] = Color.FromArgb("#ACACAC");
        Resources["Gray400"] = Color.FromArgb("#919191");
        Resources["Gray500"] = Color.FromArgb("#6E6E6E");
        Resources["Gray600"] = Color.FromArgb("#404040");
        Resources["Gray900"] = Color.FromArgb("#212121");
        Resources["Gray950"] = Color.FromArgb("#141414");

        Resources["PrimaryBrush"] = new SolidColorBrush(primary);
    }
}
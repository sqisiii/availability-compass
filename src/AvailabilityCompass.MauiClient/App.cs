using AvailabilityCompass.MauiClient.Infrastructure.Initialization;
using AvailabilityCompass.MauiClient.Pages;

namespace AvailabilityCompass.MauiClient;

public class App : Application
{
    private readonly AppViewModel _appViewModel;
    private readonly MauiBootstrapper _bootstrapper;

    public App(MauiBootstrapper bootstrapper, AppViewModel appViewModel)
    {
        _bootstrapper = bootstrapper;
        _appViewModel = appViewModel;

        // Load default styles
        Resources.Add(new ResourceDictionary { Source = new Uri("Resources/Styles/Colors.xaml", UriKind.Relative) });
        Resources.Add(new ResourceDictionary { Source = new Uri("Resources/Styles/Styles.xaml", UriKind.Relative) });
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = new AppShell();
        var window = new Window(shell);

        shell.Loaded += async (_, _) =>
        {
            await _bootstrapper.RunAsync();
            await _appViewModel.InitializeAsync();
        };

        return window;
    }
}
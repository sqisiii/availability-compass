using AvailabilityCompass.Core.Application.Database;
using AvailabilityCompass.WpfClient.Pages;
using Serilog;
using System.Runtime.InteropServices;

namespace AvailabilityCompass.WpfClient.Application.Initialization;

/// <summary>
/// Responsible for initializing and starting the application.
/// </summary>
public class Bootstrapper
{
    private readonly IDbInitializer _dbInitializer;
    private readonly MainViewModel _mainViewModel;
    private readonly MainWindow _mainWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
    /// </summary>
    /// <param name="mainWindow">The main window of the application.</param>
    /// <param name="mainViewModel">The main view model of the application.</param>
    /// <param name="dbInitializer">The database initializer.</param>
    public Bootstrapper(MainWindow mainWindow, MainViewModel mainViewModel, IDbInitializer dbInitializer)
    {
        _mainWindow = mainWindow;
        _mainViewModel = mainViewModel;
        _dbInitializer = dbInitializer;
    }

    /// <summary>
    /// Runs the bootstrapper to initialize and start the application.
    /// </summary>
    public void Run()
    {
        Introduce();
        _dbInitializer.InitializeAsync().Wait();

        _mainViewModel.Initialize();
        _mainWindow.Show();
    }

    /// <summary>
    /// Logs introductory information about the application and runtime environment.
    /// </summary>
    private void Introduce()
    {
        Log.Information("");
        Log.Information("");
        Log.Information("-----------------------------------------------------------------------------------------");
        Log.Information("Application starting up. Version: {version}", typeof(App).Assembly.GetName().Version);

        Log.Information($"Runtime data: .Net version: {RuntimeInformation.FrameworkDescription}; OS description: {RuntimeInformation.OSDescription}; OS Architecture: {RuntimeInformation.OSArchitecture}; Process Architecture: {RuntimeInformation.ProcessArchitecture}; Runtime Identifier: {RuntimeInformation.RuntimeIdentifier};");
    }
}

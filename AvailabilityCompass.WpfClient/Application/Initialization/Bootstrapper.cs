using System.Runtime.InteropServices;
using AvailabilityCompass.Core.Application.Database;
using Serilog;

namespace AvailabilityCompass.WpfClient.Application.Initialization;

public class Bootstrapper
{
    private readonly IDbInitializer _dbInitializer;
    private readonly MainWindow _mainWindow;

    public Bootstrapper(MainWindow mainWindow, IDbInitializer dbInitializer)
    {
        _mainWindow = mainWindow;
        _dbInitializer = dbInitializer;
    }

    public void Run()
    {
        Introduce();
        _dbInitializer.InitializeAsync().Wait();

        _mainWindow.Show();
    }

    private void Introduce()
    {
        Log.Information("");
        Log.Information("");
        Log.Information("-----------------------------------------------------------------------------------------");
        Log.Information("Application starting up. Version: {version}", typeof(App).Assembly.GetName().Version);

        Log.Information($"Runtime data: .Net version: {RuntimeInformation.FrameworkDescription}; OS description: {RuntimeInformation.OSDescription}; OS Architecture: {RuntimeInformation.OSArchitecture}; Process Architecture: {RuntimeInformation.ProcessArchitecture}; Runtime Identifier: {RuntimeInformation.RuntimeIdentifier};");
    }
}
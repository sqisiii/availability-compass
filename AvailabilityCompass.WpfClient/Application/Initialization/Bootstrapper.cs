using System.Runtime.InteropServices;
using AvailabilityCompass.WpfClient.Pages;
using Serilog;

namespace AvailabilityCompass.WpfClient.Application.Initialization;

public class Bootstrapper
{
    private readonly MainWindow _mainWindow;


    public Bootstrapper(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public void Run()
    {
        Introduce();
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
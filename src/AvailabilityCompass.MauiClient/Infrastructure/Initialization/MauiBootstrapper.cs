using System.Runtime.InteropServices;
using AvailabilityCompass.Core.Application.Database;
using AvailabilityCompass.Core.Features.ManageSettings;
using Serilog;

namespace AvailabilityCompass.MauiClient.Infrastructure.Initialization;

public class MauiBootstrapper
{
    private readonly IDbInitializer _dbInitializer;
    private readonly IThemeService _themeService;

    public MauiBootstrapper(IDbInitializer dbInitializer, IThemeService themeService)
    {
        _dbInitializer = dbInitializer;
        _themeService = themeService;
    }

    public async Task RunAsync()
    {
        Introduce();
        await _dbInitializer.InitializeAsync();
        await _themeService.LoadThemeAsync();
    }

    private static void Introduce()
    {
        Log.Information("");
        Log.Information("-----------------------------------------------------------------------------------------");
        Log.Information("MAUI Application starting up. Version: {Version}",
            typeof(MauiBootstrapper).Assembly.GetName().Version);
        Log.Information(
            "Runtime: .NET {Framework}; OS: {OS}; Arch: {Arch}; RID: {RID}",
            RuntimeInformation.FrameworkDescription,
            RuntimeInformation.OSDescription,
            RuntimeInformation.OSArchitecture,
            RuntimeInformation.RuntimeIdentifier);
    }
}
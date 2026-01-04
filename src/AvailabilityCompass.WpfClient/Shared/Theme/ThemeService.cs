using System.Windows;
using AvailabilityCompass.Core.Features.ManageSettings;
using AvailabilityCompass.Core.Features.ManageSettings.Commands.SaveSetting;
using AvailabilityCompass.Core.Features.ManageSettings.Queries.GetSetting;
using MediatR;
using WpfApp = System.Windows.Application;

namespace AvailabilityCompass.WpfClient.Shared.Theme;

public class ThemeService : IThemeService
{
    private const string ThemeSettingKey = "Theme";
    private const string DarkThemeValue = "Dark";
    private const string LightThemeValue = "Light";

    private readonly IMediator _mediator;

    public ThemeService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public bool IsDarkTheme { get; private set; }

    public async Task LoadThemeAsync()
    {
        var response = await _mediator.Send(new GetSettingQuery(ThemeSettingKey, LightThemeValue));
        IsDarkTheme = response.Value == DarkThemeValue;
        ApplyTheme(IsDarkTheme);
    }

    public async Task SaveThemeAsync(bool isDark)
    {
        IsDarkTheme = isDark;
        ApplyTheme(isDark);

        var value = isDark ? DarkThemeValue : LightThemeValue;
        await _mediator.Send(new SaveSettingRequest(ThemeSettingKey, value));
    }

    private static void ApplyTheme(bool isDark)
    {
        WpfApp.Current.Dispatcher.Invoke(() => { UpdateGlassTheme(isDark); });
    }

    private static void UpdateGlassTheme(bool isDark)
    {
        var resources = WpfApp.Current.Resources;
        var darkTheme = new Uri("pack://application:,,,/AvailabilityCompass.WpfClient;component/Themes/GlassDarkTheme.xaml",
            UriKind.Absolute);
        var lightTheme = new Uri("pack://application:,,,/AvailabilityCompass.WpfClient;component/Themes/GlassLightTheme.xaml",
            UriKind.Absolute);

        // Find ALL theme dictionaries (match by filename, not full URI)
        // This handles both relative paths from App.xaml and absolute pack:// URIs
        var themesToRemove = resources.MergedDictionaries
            .Where(dict => dict.Source?.OriginalString.EndsWith("GlassLightTheme.xaml") == true ||
                           dict.Source?.OriginalString.EndsWith("GlassDarkTheme.xaml") == true)
            .ToList();

        var newTheme = new ResourceDictionary
        {
            Source = isDark ? darkTheme : lightTheme
        };

        // Insert new theme FIRST to ensure resources are always available
        resources.MergedDictionaries.Insert(0, newTheme);

        // Then remove old themes
        foreach (var theme in themesToRemove)
        {
            resources.MergedDictionaries.Remove(theme);
        }
    }
}
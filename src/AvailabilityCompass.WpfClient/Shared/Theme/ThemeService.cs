using System.Windows;
using AvailabilityCompass.Core.Features.ManageSettings;
using AvailabilityCompass.Core.Features.ManageSettings.Commands.SaveSetting;
using AvailabilityCompass.Core.Features.ManageSettings.Queries.GetSetting;
using MaterialDesignThemes.Wpf;
using MediatR;

namespace AvailabilityCompass.WpfClient.Shared.Theme;

public class ThemeService : IThemeService
{
    private const string ThemeSettingKey = "Theme";
    private const string DarkThemeValue = "Dark";
    private const string LightThemeValue = "Light";

    private readonly IMediator _mediator;
    private readonly PaletteHelper _paletteHelper;
    private bool _isDarkTheme;

    public ThemeService(IMediator mediator)
    {
        _mediator = mediator;
        _paletteHelper = new PaletteHelper();
    }

    public bool IsDarkTheme => _isDarkTheme;

    public async Task LoadThemeAsync()
    {
        var response = await _mediator.Send(new GetSettingQuery(ThemeSettingKey, LightThemeValue));
        _isDarkTheme = response.Value == DarkThemeValue;
        ApplyTheme(_isDarkTheme);
    }

    public async Task SaveThemeAsync(bool isDark)
    {
        _isDarkTheme = isDark;
        ApplyTheme(isDark);

        var value = isDark ? DarkThemeValue : LightThemeValue;
        await _mediator.Send(new SaveSettingRequest(ThemeSettingKey, value));
    }

    private void ApplyTheme(bool isDark)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            var theme = _paletteHelper.GetTheme();
            theme.SetBaseTheme(isDark ? BaseTheme.Dark : BaseTheme.Light);
            _paletteHelper.SetTheme(theme);

            UpdateShadcnTheme(isDark);
        });
    }

    private void UpdateShadcnTheme(bool isDark)
    {
        var resources = System.Windows.Application.Current.Resources;
        var lightTheme = new Uri("pack://application:,,,/AvailabilityCompass.WpfClient;component/Themes/ShadcnLightTheme.xaml", UriKind.Absolute);
        var darkTheme = new Uri("pack://application:,,,/AvailabilityCompass.WpfClient;component/Themes/ShadcnDarkTheme.xaml", UriKind.Absolute);

        ResourceDictionary? existingTheme = null;
        foreach (var dict in resources.MergedDictionaries)
        {
            if (dict.Source == lightTheme || dict.Source == darkTheme)
            {
                existingTheme = dict;
                break;
            }
        }

        if (existingTheme is not null)
        {
            resources.MergedDictionaries.Remove(existingTheme);
        }

        var newTheme = new ResourceDictionary
        {
            Source = isDark ? darkTheme : lightTheme
        };
        resources.MergedDictionaries.Add(newTheme);
    }
}

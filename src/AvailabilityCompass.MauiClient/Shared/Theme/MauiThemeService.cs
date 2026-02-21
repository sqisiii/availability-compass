using AvailabilityCompass.Core.Features.ManageSettings;
using AvailabilityCompass.Core.Features.ManageSettings.Commands.SaveSetting;
using AvailabilityCompass.Core.Features.ManageSettings.Queries.GetSetting;
using MediatR;

namespace AvailabilityCompass.MauiClient.Shared.Theme;

public class MauiThemeService : IThemeService
{
    private const string ThemeSettingKey = "Theme";
    private const string DarkThemeValue = "Dark";
    private const string LightThemeValue = "Light";

    private readonly IMediator _mediator;

    public MauiThemeService(IMediator mediator)
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
        if (Application.Current is null)
            return;

        Application.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
    }
}
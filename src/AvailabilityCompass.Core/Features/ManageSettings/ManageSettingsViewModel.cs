using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageSettings;

/// <summary>
/// View model for managing application settings.
/// </summary>
public partial class ManageSettingsViewModel : ObservableValidator, IPageViewModel
{
    private readonly IThemeService _themeService;

    [ObservableProperty]
    private bool _isThemeDark;

    public ManageSettingsViewModel(IThemeService themeService)
    {
        _themeService = themeService;
        _isThemeDark = themeService.IsDarkTheme;
    }

    public bool IsActive { get; set; }
    public string Icon => "CogOutline";

    public string Name => "Settings";

    public Task LoadDataAsync(CancellationToken ct)
    {
        IsThemeDark = _themeService.IsDarkTheme;
        return Task.CompletedTask;
    }

    partial void OnIsThemeDarkChanged(bool value)
    {
        _ = _themeService.SaveThemeAsync(value);
    }
}
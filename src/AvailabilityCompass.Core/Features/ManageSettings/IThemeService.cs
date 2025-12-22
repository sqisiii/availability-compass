namespace AvailabilityCompass.Core.Features.ManageSettings;

public interface IThemeService
{
    bool IsDarkTheme { get; }
    Task LoadThemeAsync();
    Task SaveThemeAsync(bool isDark);
}

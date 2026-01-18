namespace AvailabilityCompass.Core.Features.ManageSettings;

/// <summary>
/// Service interface for managing application theme settings.
/// </summary>
public interface IThemeService
{
    bool IsDarkTheme { get; }

    /// <summary>
    /// Loads the theme setting from persistent storage.
    /// </summary>
    /// <returns>A task representing the asynchronous load operation.</returns>
    Task LoadThemeAsync();

    /// <summary>
    /// Saves the theme setting to persistent storage.
    /// </summary>
    /// <param name="isDark">True to save dark theme, false for light theme.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task SaveThemeAsync(bool isDark);
}

namespace AvailabilityCompass.Core.Shared;

/// <summary>
/// Represents a view model for a page.
/// </summary>
public interface IPageViewModel
{
    /// <summary>
    /// Gets or sets a value indicating whether the page is active.
    /// </summary>
    bool IsActive { get; set; }

    /// <summary>
    /// Gets the icon associated with the page to be shown in the menu.
    /// </summary>
    string Icon { get; }

    /// <summary>
    /// Gets the name of the page to be shown in the menu
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Asynchronously loads data for the page.
    /// </summary>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task LoadDataAsync(CancellationToken ct);
}

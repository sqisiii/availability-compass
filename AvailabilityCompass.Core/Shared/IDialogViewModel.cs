namespace AvailabilityCompass.Core.Shared;

/// <summary>
/// Represents a view model for a dialog with properties to manage its state.
/// </summary>
public interface IDialogViewModel
{
    /// <summary>
    /// Gets or sets a value indicating whether the dialog is active.
    /// </summary>
    bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog is open.
    /// </summary>
    bool IsDialogOpen { get; set; }
}

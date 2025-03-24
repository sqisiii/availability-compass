namespace AvailabilityCompass.Core.Shared.Navigation;

/// <summary>
/// Interface for a navigation store that holds the current view model.
/// </summary>
/// <typeparam name="T">The type of the view model.</typeparam>
public interface INavigationStore<T>
{
    /// <summary>
    /// Gets or sets the current view model.
    /// </summary>
    T? CurrentViewModel { get; set; }

    /// <summary>
    /// Event that is triggered when the current view model changes.
    /// </summary>
    event Action? CurrentViewModelChanged;
}

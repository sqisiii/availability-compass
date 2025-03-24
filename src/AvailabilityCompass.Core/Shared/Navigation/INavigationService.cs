namespace AvailabilityCompass.Core.Shared.Navigation;

/// <summary>
/// Defines a service for navigating between views and dialogs.
/// </summary>
/// <typeparam name="T">The type of the view model.</typeparam>
public interface INavigationService<in T>
{
    /// <summary>
    /// Navigates to the specified view model.
    /// </summary>
    /// <param name="viewModel">The view model to navigate to.</param>
    void NavigateTo(T viewModel);

    /// <summary>
    /// Closes the current view.
    /// </summary>
    void CloseView();
}
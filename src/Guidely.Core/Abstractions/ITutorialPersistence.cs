namespace Guidely.Core.Abstractions;

/// <summary>
/// Interface for persisting tutorial state.
/// Consumers implement this to store state in their preferred storage mechanism.
/// </summary>
public interface ITutorialPersistence
{
    /// <summary>
    /// Loads the persisted tutorial state.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The persisted state, or null if no state exists.</returns>
    Task<TutorialState?> LoadStateAsync(CancellationToken ct = default);

    /// <summary>
    /// Saves the tutorial state.
    /// </summary>
    /// <param name="state">The state to persist.</param>
    /// <param name="ct">Cancellation token.</param>
    Task SaveStateAsync(TutorialState state, CancellationToken ct = default);
}
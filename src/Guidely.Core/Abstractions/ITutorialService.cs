using System.ComponentModel;

namespace Guidely.Core.Abstractions;

/// <summary>
/// Service interface for managing the tutorial state machine.
/// </summary>
/// <typeparam name="TContext">The tutorial context type.</typeparam>
/// <typeparam name="TTrigger">The trigger enum type.</typeparam>
/// <typeparam name="TGroup">The group enum type.</typeparam>
public interface ITutorialService<TContext, TTrigger, TGroup> : INotifyPropertyChanged
    where TContext : TutorialContextBase
    where TTrigger : Enum
    where TGroup : Enum
{
    /// <summary>
    /// The ID of the current step.
    /// </summary>
    string CurrentStepId { get; }

    /// <summary>
    /// The group of the current step.
    /// </summary>
    TGroup CurrentGroup { get; }

    /// <summary>
    /// Metadata for the current step, or null if tutorial is not active.
    /// </summary>
    TutorialStepMetadata<TTrigger, TGroup>? CurrentStep { get; }

    /// <summary>
    /// Whether the tutorial is currently active.
    /// </summary>
    bool IsTutorialActive { get; }

    /// <summary>
    /// Whether the tutorial has been completed.
    /// </summary>
    bool IsTutorialCompleted { get; }

    /// <summary>
    /// The current tutorial context.
    /// </summary>
    TContext Context { get; }

    /// <summary>
    /// The current step number (1-based).
    /// </summary>
    int CurrentStepNumber { get; }

    /// <summary>
    /// The total number of steps in the tutorial.
    /// </summary>
    int TotalSteps { get; }

    /// <summary>
    /// Whether back navigation is available.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Whether the current step can be skipped (implements ITutorialStepSkippable and CanSkip returns true).
    /// </summary>
    bool IsCurrentStepSkippable { get; }

    /// <summary>
    /// Initializes the tutorial service, loading any persisted state.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    Task InitializeAsync(CancellationToken ct = default);

    /// <summary>
    /// Starts or restarts the tutorial from the beginning.
    /// </summary>
    void StartTutorial();

    /// <summary>
    /// Advances to the next step.
    /// </summary>
    void AdvanceStep();

    /// <summary>
    /// Goes back to the previous step.
    /// </summary>
    void GoBack();

    /// <summary>
    /// Skips the tutorial entirely.
    /// </summary>
    void SkipTutorial();

    /// <summary>
    /// Restarts the tutorial from the beginning, clearing all progress.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    Task RestartTutorialAsync(CancellationToken ct = default);

    /// <summary>
    /// Fires a trigger with an optional context update (atomic operation).
    /// </summary>
    /// <param name="trigger">The trigger to fire.</param>
    /// <param name="contextUpdate">Optional function to update the context.</param>
    void FireTrigger(TTrigger trigger, Func<TContext, TContext>? contextUpdate = null);

    /// <summary>
    /// Updates the context without firing a trigger.
    /// </summary>
    /// <param name="update">Function to update the context.</param>
    void UpdateContext(Func<TContext, TContext> update);

    /// <summary>
    /// Restarts the tutorial from the beginning of the specified group.
    /// </summary>
    /// <param name="group">The group to restart from.</param>
    void RestartGroup(TGroup group);

    /// <summary>
    /// Navigates using the configured skip transition for the current step.
    /// </summary>
    void SkipStep();

    /// <summary>
    /// Restarts the tutorial from the group matching the current view/page.
    /// Falls back to full restart if no group matches.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    Task RestartFromCurrentViewAsync(CancellationToken ct = default);
}
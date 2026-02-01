using System.ComponentModel;

namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Service managing the tutorial state machine, persistence, and flow control.
/// </summary>
public interface ITutorialService : INotifyPropertyChanged
{
    /// <summary>
    /// The current step in the tutorial.
    /// </summary>
    TutorialStepId CurrentStepId { get; }

    /// <summary>
    /// The definition for the current step, or null if the tutorial is inactive.
    /// </summary>
    TutorialStepDefinition? CurrentStep { get; }

    /// <summary>
    /// Whether the tutorial is currently active and showing.
    /// </summary>
    bool IsTutorialActive { get; }

    /// <summary>
    /// Whether the tutorial has been completed at least once.
    /// </summary>
    bool IsTutorialCompleted { get; }

    /// <summary>
    /// The current application context for conditional flow.
    /// </summary>
    TutorialContext Context { get; }

    /// <summary>
    /// Current step index (1-based) for progress display.
    /// </summary>
    int CurrentStepNumber { get; }

    /// <summary>
    /// Total number of steps for progress display.
    /// </summary>
    int TotalSteps { get; }

    /// <summary>
    /// Initializes the tutorial service, loading saved state from settings.
    /// </summary>
    Task InitializeAsync(CancellationToken ct = default);

    /// <summary>
    /// Starts or restarts the tutorial from the beginning.
    /// </summary>
    void StartTutorial();

    /// <summary>
    /// Advances to the next appropriate step based on the current context.
    /// </summary>
    void AdvanceStep();

    /// <summary>
    /// Goes back to the previous step if possible.
    /// </summary>
    void GoBack();

    /// <summary>
    /// Skips the tutorial entirely.
    /// </summary>
    void SkipTutorial();

    /// <summary>
    /// Restarts the tutorial, clearing all progress.
    /// </summary>
    Task RestartTutorialAsync(CancellationToken ct = default);

    /// <summary>
    /// Updates the application context and potentially triggers step changes.
    /// </summary>
    void UpdateContext(TutorialContext context);
}
namespace Guidely.Core.Abstractions;

/// <summary>
/// Persisted state of the tutorial.
/// </summary>
/// <param name="IsActive">Whether the tutorial is currently active.</param>
/// <param name="CurrentStepId">The ID of the current step.</param>
/// <param name="IsCompleted">Whether the tutorial has been completed.</param>
/// <param name="IsSkipped">Whether the tutorial has been skipped.</param>
/// <param name="StepStatuses">Per-step status tracking.</param>
public record TutorialState(
    bool IsActive,
    string CurrentStepId,
    bool IsCompleted,
    bool IsSkipped,
    Dictionary<string, StepStatus> StepStatuses
);

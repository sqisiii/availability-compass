namespace Guidely.Core.Abstractions;

/// <summary>
/// Persisted state of the tutorial.
/// </summary>
/// <param name="IsCompleted">Whether the tutorial has been completed.</param>
/// <param name="IsSkipped">Whether the tutorial has been skipped.</param>
public record TutorialState(
    bool IsCompleted,
    bool IsSkipped
);

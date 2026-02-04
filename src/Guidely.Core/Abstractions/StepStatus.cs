namespace Guidely.Core.Abstractions;

/// <summary>
/// Status of a tutorial step.
/// </summary>
public enum StepStatus
{
    /// <summary>
    /// Step has not been shown yet.
    /// </summary>
    NotVisited,

    /// <summary>
    /// Step was shown but not completed.
    /// </summary>
    Visited,

    /// <summary>
    /// Step was completed (user clicked Next or auto-advanced).
    /// </summary>
    Completed
}
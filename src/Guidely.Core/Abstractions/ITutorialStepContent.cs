namespace Guidely.Core.Abstractions;

/// <summary>
/// Interface for providing step content.
/// </summary>
public interface ITutorialStepContent
{
    /// <summary>
    /// The title of the step.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// The description/body of the step. Supports markdown.
    /// </summary>
    string Description { get; }
}
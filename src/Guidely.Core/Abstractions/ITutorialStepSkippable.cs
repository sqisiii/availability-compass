namespace Guidely.Core.Abstractions;

/// <summary>
/// Interface for steps that can be skipped when their goal is already achieved.
/// When <see cref="CanSkip"/> returns true, the step shows alternate content
/// and the Next button appears (even if RequiresUserAction is true),
/// allowing the user to skip to a configured target step.
/// </summary>
/// <typeparam name="TContext">The tutorial context type.</typeparam>
public interface ITutorialStepSkippable<in TContext> where TContext : TutorialContextBase
{
    /// <summary>
    /// The title to display when the step is skippable.
    /// </summary>
    string SkipTitle { get; }

    /// <summary>
    /// The description to display when the step is skippable.
    /// </summary>
    string SkipDescription { get; }

    /// <summary>
    /// Returns true if the step can be skipped (its goal is already achieved).
    /// </summary>
    /// <param name="context">The current tutorial context.</param>
    /// <returns>True if the step can be skipped, false otherwise.</returns>
    bool CanSkip(TContext context);
}
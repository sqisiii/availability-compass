namespace Guidely.Core.Abstractions;

/// <summary>
/// Interface for steps that can be auto-skipped when their goal is already achieved.
/// Used when restarting at group start to skip already-completed steps.
/// </summary>
/// <typeparam name="TContext">The tutorial context type.</typeparam>
public interface ITutorialStepComplete<in TContext> where TContext : TutorialContextBase
{
    /// <summary>
    /// Returns true if the step's goal is already achieved.
    /// When restarting a group, steps where IsComplete returns true will be auto-skipped.
    /// </summary>
    /// <param name="context">The current tutorial context.</param>
    /// <returns>True if the step is complete, false otherwise.</returns>
    bool IsComplete(TContext context);
}
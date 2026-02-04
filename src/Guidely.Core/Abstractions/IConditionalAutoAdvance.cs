namespace Guidely.Core.Abstractions;

/// <summary>
/// Interface for steps that need conditional logic for auto-advancing.
/// </summary>
/// <typeparam name="TContext">The tutorial context type.</typeparam>
/// <typeparam name="TTrigger">The trigger enum type.</typeparam>
public interface IConditionalAutoAdvance<in TContext, in TTrigger>
    where TContext : TutorialContextBase
    where TTrigger : Enum
{
    /// <summary>
    /// Determines whether the step should auto-advance based on the trigger and context.
    /// </summary>
    /// <param name="trigger">The trigger that was fired.</param>
    /// <param name="previousContext">The context before the update (null if no previous context).</param>
    /// <param name="currentContext">The current context after the update.</param>
    /// <returns>True if the step should auto-advance, false otherwise.</returns>
    bool ShouldAutoAdvance(TTrigger trigger, TContext? previousContext, TContext currentContext);
}
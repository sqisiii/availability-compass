namespace Guidely.Core.Abstractions;

/// <summary>
/// Defines a transition rule from one step to another.
/// </summary>
/// <param name="FromStepId">The source step ID.</param>
/// <param name="ToStepId">The target step ID.</param>
/// <param name="Condition">Optional condition function that determines if this transition applies.</param>
public record TransitionRule(
    string FromStepId,
    string ToStepId,
    Func<object, bool>? Condition = null
);
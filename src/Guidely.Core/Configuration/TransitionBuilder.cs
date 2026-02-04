using Guidely.Core.Abstractions;

namespace Guidely.Core.Configuration;

/// <summary>
/// Fluent builder for configuring step transitions.
/// </summary>
/// <typeparam name="TContext">The tutorial context type.</typeparam>
public class TransitionBuilder<TContext> where TContext : TutorialContextBase
{
    private readonly List<TransitionRule> _rules = [];
    private string? _currentFromStep;

    /// <summary>
    /// Specifies the source step for transitions.
    /// </summary>
    /// <param name="stepId">The source step ID.</param>
    /// <returns>The builder for chaining.</returns>
    public TransitionBuilder<TContext> From(string stepId)
    {
        _currentFromStep = stepId;
        return this;
    }

    /// <summary>
    /// Adds an unconditional transition to the target step.
    /// </summary>
    /// <param name="stepId">The target step ID.</param>
    /// <returns>The builder for chaining.</returns>
    public TransitionBuilder<TContext> GoTo(string stepId)
    {
        EnsureFromStepSet();
        _rules.Add(new TransitionRule(_currentFromStep!, stepId));
        return this;
    }

    /// <summary>
    /// Adds a conditional transition to the target step.
    /// </summary>
    /// <param name="stepId">The target step ID.</param>
    /// <param name="condition">The condition that must be true for this transition.</param>
    /// <returns>The builder for chaining.</returns>
    public TransitionBuilder<TContext> GoToIf(string stepId, Func<TContext, bool> condition)
    {
        EnsureFromStepSet();
        _rules.Add(new TransitionRule(_currentFromStep!, stepId, ctx => condition((TContext)ctx)));
        return this;
    }

    /// <summary>
    /// Builds the list of transition rules.
    /// </summary>
    /// <returns>The configured transition rules.</returns>
    internal IReadOnlyList<TransitionRule> Build() => _rules.AsReadOnly();

    private void EnsureFromStepSet()
    {
        if (string.IsNullOrEmpty(_currentFromStep))
        {
            throw new InvalidOperationException("Must call From() before GoTo() or GoToIf()");
        }
    }
}
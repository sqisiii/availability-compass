using Guidely.Core.Abstractions;

namespace Guidely.Core.Configuration;

/// <summary>
/// Fluent builder for configuring step transitions.
/// </summary>
/// <typeparam name="TContext">The tutorial context type.</typeparam>
public class TransitionBuilder<TContext> where TContext : TutorialContextBase
{
    private readonly Dictionary<string, string> _backTransitions = new();
    private readonly HashSet<string> _noBackSteps = [];
    private readonly List<TransitionRule> _rules = [];
    private readonly Dictionary<string, string> _skipTransitions = new();
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
    /// Configures the skip target step for the current source step.
    /// When the step implements <see cref="Abstractions.ITutorialStepSkippable{TContext}"/>
    /// and CanSkip returns true, clicking Next will navigate to this target.
    /// </summary>
    /// <param name="stepId">The target step ID to skip to.</param>
    /// <returns>The builder for chaining.</returns>
    public TransitionBuilder<TContext> SkipTo(string stepId)
    {
        EnsureFromStepSet();
        _skipTransitions[_currentFromStep!] = stepId;
        return this;
    }

    /// <summary>
    /// Configures an explicit back target for the current source step.
    /// When GoBack is called on this step, it navigates directly to the target
    /// instead of using history-based back navigation.
    /// </summary>
    /// <param name="stepId">The target step ID to go back to.</param>
    /// <returns>The builder for chaining.</returns>
    public TransitionBuilder<TContext> BackTo(string stepId)
    {
        EnsureFromStepSet();
        _backTransitions[_currentFromStep!] = stepId;
        return this;
    }

    /// <summary>
    /// Disables back navigation for the current source step.
    /// The back button will be hidden when this step is active.
    /// </summary>
    /// <returns>The builder for chaining.</returns>
    public TransitionBuilder<TContext> DisableBack()
    {
        EnsureFromStepSet();
        _noBackSteps.Add(_currentFromStep!);
        return this;
    }

    /// <summary>
    /// Builds the list of transition rules.
    /// </summary>
    /// <returns>The configured transition rules.</returns>
    internal IReadOnlyList<TransitionRule> Build() => _rules.AsReadOnly();

    /// <summary>
    /// Builds the skip transitions dictionary.
    /// </summary>
    /// <returns>The configured skip transitions.</returns>
    internal IReadOnlyDictionary<string, string> BuildSkipTransitions() =>
        new Dictionary<string, string>(_skipTransitions);

    /// <summary>
    /// Builds the back transitions dictionary.
    /// </summary>
    /// <returns>The configured back transitions.</returns>
    internal IReadOnlyDictionary<string, string> BuildBackTransitions() =>
        new Dictionary<string, string>(_backTransitions);

    /// <summary>
    /// Builds the set of steps with back navigation disabled.
    /// </summary>
    /// <returns>The set of step IDs with back disabled.</returns>
    internal IReadOnlySet<string> BuildNoBackSteps() =>
        new HashSet<string>(_noBackSteps);

    private void EnsureFromStepSet()
    {
        if (string.IsNullOrEmpty(_currentFromStep))
        {
            throw new InvalidOperationException(
                "Must call From() before GoTo(), GoToIf(), SkipTo(), BackTo(), or DisableBack()");
        }
    }
}
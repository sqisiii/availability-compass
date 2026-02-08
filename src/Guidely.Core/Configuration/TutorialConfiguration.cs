using Guidely.Core.Abstractions;

namespace Guidely.Core.Configuration;

/// <summary>
/// Complete configuration for the tutorial system.
/// </summary>
/// <typeparam name="TContext">The tutorial context type.</typeparam>
/// <typeparam name="TTrigger">The trigger enum type.</typeparam>
/// <typeparam name="TGroup">The group enum type.</typeparam>
public class TutorialConfiguration<TContext, TTrigger, TGroup>
    where TContext : TutorialContextBase, new()
    where TTrigger : Enum
    where TGroup : Enum
{
    /// <summary>
    /// The ID of the starting step.
    /// </summary>
    public string StartStepId { get; internal init; } = string.Empty;

    /// <summary>
    /// All registered step metadata.
    /// </summary>
    public IReadOnlyDictionary<string, TutorialStepMetadata<TTrigger, TGroup>> Steps { get; internal init; }
        = new Dictionary<string, TutorialStepMetadata<TTrigger, TGroup>>();

    /// <summary>
    /// Steps ordered by group and order within a group.
    /// </summary>
    public IReadOnlyList<TutorialStepMetadata<TTrigger, TGroup>> OrderedSteps { get; internal init; }
        = Array.Empty<TutorialStepMetadata<TTrigger, TGroup>>();

    /// <summary>
    /// Transition rules defining navigation between steps.
    /// </summary>
    public IReadOnlyList<TransitionRule> Transitions { get; internal init; }
        = Array.Empty<TransitionRule>();

    /// <summary>
    /// Skip transitions mapping step ID to skip target step ID.
    /// Used when a step implements <see cref="Abstractions.ITutorialStepSkippable{TContext}"/>
    /// and CanSkip returns true.
    /// </summary>
    public IReadOnlyDictionary<string, string> SkipTransitions { get; internal init; }
        = new Dictionary<string, string>();

    /// <summary>
    /// Maps tutorial groups to view/page conditions for restart-from-current-view behavior.
    /// </summary>
    public IReadOnlyDictionary<TGroup, Func<TContext, bool>> GroupViewMappings { get; internal init; }
        = new Dictionary<TGroup, Func<TContext, bool>>();

    /// <summary>
    /// Whether to automatically start the tutorial on the first run (no persisted state).
    /// </summary>
    public bool AutoStartOnFirstRun { get; internal init; }

    /// <summary>
    /// Gets the first step of a group.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <returns>The first step in the group, or null if a group has no steps.</returns>
    public TutorialStepMetadata<TTrigger, TGroup>? GetFirstStepInGroup(TGroup group)
    {
        return OrderedSteps.FirstOrDefault(s => EqualityComparer<TGroup>.Default.Equals(s.Group, group));
    }

    /// <summary>
    /// Gets all steps in a group.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <returns>All steps in the group, ordered.</returns>
    public IEnumerable<TutorialStepMetadata<TTrigger, TGroup>> GetStepsInGroup(TGroup group)
    {
        return OrderedSteps.Where(s => EqualityComparer<TGroup>.Default.Equals(s.Group, group));
    }
}
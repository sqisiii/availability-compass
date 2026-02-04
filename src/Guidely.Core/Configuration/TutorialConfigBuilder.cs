using System.Reflection;
using Guidely.Core.Abstractions;

namespace Guidely.Core.Configuration;

/// <summary>
/// Builder for configuring the tutorial system.
/// </summary>
/// <typeparam name="TContext">The tutorial context type.</typeparam>
/// <typeparam name="TTrigger">The trigger enum type.</typeparam>
/// <typeparam name="TGroup">The group enum type.</typeparam>
public class TutorialConfigBuilder<TContext, TTrigger, TGroup>
    where TContext : TutorialContextBase, new()
    where TTrigger : Enum
    where TGroup : Enum
{
    private readonly List<Assembly> _assemblies = [];
    private readonly TransitionBuilder<TContext> _transitionBuilder = new();
    private bool _autoStartOnFirstRun;
    private string _startStepId = string.Empty;
    private Func<IServiceProvider, Type, object>? _stepFactory;

    /// <summary>
    /// Scans an assembly for step classes marked with [TutorialStep].
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>The builder for chaining.</returns>
    public TutorialConfigBuilder<TContext, TTrigger, TGroup> ScanStepsFromAssembly(Assembly assembly)
    {
        _assemblies.Add(assembly);
        return this;
    }

    /// <summary>
    /// Configures transitions between steps.
    /// </summary>
    /// <param name="configure">Action to configure transitions.</param>
    /// <returns>The builder for chaining.</returns>
    public TutorialConfigBuilder<TContext, TTrigger, TGroup> ConfigureTransitions(
        Action<TransitionBuilder<TContext>> configure)
    {
        configure(_transitionBuilder);
        return this;
    }

    /// <summary>
    /// Sets the starting step ID.
    /// </summary>
    /// <param name="stepId">The ID of the first step.</param>
    /// <returns>The builder for chaining.</returns>
    public TutorialConfigBuilder<TContext, TTrigger, TGroup> SetStartStep(string stepId)
    {
        _startStepId = stepId;
        return this;
    }

    /// <summary>
    /// Sets a custom factory for creating step instances.
    /// </summary>
    /// <param name="factory">Factory function that takes service provider and step type.</param>
    /// <returns>The builder for chaining.</returns>
    // ReSharper disable once UnusedMember.Global
    public TutorialConfigBuilder<TContext, TTrigger, TGroup> UseStepFactory(
        Func<IServiceProvider, Type, object> factory)
    {
        _stepFactory = factory;
        return this;
    }

    /// <summary>
    /// Enables automatic tutorial start on the first run when no persisted state exists.
    /// </summary>
    /// <returns>The builder for chaining.</returns>
    public TutorialConfigBuilder<TContext, TTrigger, TGroup> EnableAutoStart()
    {
        _autoStartOnFirstRun = true;
        return this;
    }

    /// <summary>
    /// Builds the tutorial configuration.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <returns>The built configuration.</returns>
    public TutorialConfiguration<TContext, TTrigger, TGroup> Build(IServiceProvider? serviceProvider = null)
    {
        var steps = new Dictionary<string, TutorialStepMetadata<TTrigger, TGroup>>();

        foreach (var assembly in _assemblies)
        {
            ScanAssembly(assembly, steps, serviceProvider);
        }

        var orderedSteps = steps.Values
            .OrderBy(s => Convert.ToInt32(s.Group))
            .ThenBy(s => s.Order)
            .ToList();

        return new TutorialConfiguration<TContext, TTrigger, TGroup>
        {
            StartStepId = _startStepId,
            Steps = steps,
            OrderedSteps = orderedSteps,
            Transitions = _transitionBuilder.Build(),
            AutoStartOnFirstRun = _autoStartOnFirstRun
        };
    }

    private void ScanAssembly(
        Assembly assembly,
        Dictionary<string, TutorialStepMetadata<TTrigger, TGroup>> steps,
        IServiceProvider? serviceProvider)
    {
        var stepTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<TutorialStepAttribute>() != null)
            .ToList();

        foreach (var type in stepTypes)
        {
            var stepAttr = type.GetCustomAttribute<TutorialStepAttribute>()!;
            var targetAttrs = type.GetCustomAttributes<TutorialTargetAttribute>().ToList();
            var autoAdvanceAttrs = type.GetCustomAttributes<AutoAdvanceOnAttribute>().ToList();

            // Create instance
            object instance;
            if (_stepFactory != null && serviceProvider != null)
            {
                instance = _stepFactory(serviceProvider, type);
            }
            else if (serviceProvider != null)
            {
                instance = ActivatorUtilities.CreateInstance(serviceProvider, type);
            }
            else
            {
                instance = Activator.CreateInstance(type)
                           ?? throw new InvalidOperationException($"Failed to create instance of {type.Name}");
            }

            // Get content from instance
            var title = string.Empty;
            var description = string.Empty;
            if (instance is ITutorialStepContent content)
            {
                title = content.Title;
                description = content.Description;
            }

            // Convert triggers (handle both enum and int values)
            var triggers = autoAdvanceAttrs
                .Select(a => (TTrigger)Enum.ToObject(typeof(TTrigger), Convert.ToInt32(a.Trigger)))
                .ToList();

            // Convert group (handle both enum and int values)
            var groupValue = stepAttr.Group != null ? Convert.ToInt32(stepAttr.Group) : 0;
            var group = (TGroup)Enum.ToObject(typeof(TGroup), groupValue);

            var metadata = new TutorialStepMetadata<TTrigger, TGroup>(
                Id: stepAttr.Id,
                Title: title,
                Description: description,
                Group: group,
                Order: stepAttr.Order,
                Targets: targetAttrs.Select(t => t.ElementName).ToList(),
                Position: stepAttr.Position,
                RequiresUserAction: stepAttr.RequiresUserAction,
                ClickThroughMode: stepAttr.ClickThroughMode,
                AutoAdvanceTriggers: triggers,
                StepInstance: instance
            );

            steps[stepAttr.Id] = metadata;
        }
    }
}

/// <summary>
/// Helper class for DI activation.
/// </summary>
internal static class ActivatorUtilities
{
    public static object CreateInstance(IServiceProvider serviceProvider, Type type)
    {
        var constructor = type.GetConstructors().FirstOrDefault();
        if (constructor == null)
        {
            return Activator.CreateInstance(type)
                   ?? throw new InvalidOperationException($"Failed to create instance of {type.Name}");
        }

        var parameters = constructor.GetParameters();
        var args = new object[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            args[i] = serviceProvider.GetService(parameters[i].ParameterType)
                      ?? throw new InvalidOperationException(
                          $"Cannot resolve parameter '{parameters[i].Name}' of type '{parameters[i].ParameterType.Name}' for step '{type.Name}'");
        }

        return constructor.Invoke(args);
    }
}
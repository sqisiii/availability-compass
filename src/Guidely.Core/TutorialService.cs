using CommunityToolkit.Mvvm.ComponentModel;
using Guidely.Core.Abstractions;
using Guidely.Core.Configuration;

namespace Guidely.Core;

/// <summary>
/// State machine managing the tutorial flow, persistence, and event handling.
/// </summary>
/// <typeparam name="TContext">The tutorial context type.</typeparam>
/// <typeparam name="TTrigger">The trigger enum type.</typeparam>
/// <typeparam name="TGroup">The group enum type.</typeparam>
public partial class TutorialService<TContext, TTrigger, TGroup>
    : ObservableObject, ITutorialService<TContext, TTrigger, TGroup>
    where TContext : TutorialContextBase, new()
    where TTrigger : Enum
    where TGroup : Enum
{
    private readonly TutorialConfiguration<TContext, TTrigger, TGroup> _configuration;
    private readonly ITutorialPersistence? _persistence;
    private readonly Stack<string> _stepHistory = new();
    private readonly Dictionary<string, StepStatus> _stepStatuses = new();

    [ObservableProperty]
    private TContext _context;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentStep))]
    [NotifyPropertyChangedFor(nameof(CurrentStepNumber))]
    [NotifyPropertyChangedFor(nameof(CurrentGroup))]
    private string _currentStepId;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentStep))]
    private bool _isTutorialActive;

    [ObservableProperty]
    private bool _isTutorialCompleted;

    public TutorialService(
        TutorialConfiguration<TContext, TTrigger, TGroup> configuration,
        ITutorialPersistence? persistence = null)
    {
        _configuration = configuration;
        _persistence = persistence;
        _context = new TContext();
        _currentStepId = configuration.StartStepId;

        InitializeStepStatuses();
    }

    public TutorialStepMetadata<TTrigger, TGroup>? CurrentStep =>
        IsTutorialActive && _configuration.Steps.TryGetValue(CurrentStepId, out var step)
            ? step
            : null;

    public TGroup CurrentGroup =>
        CurrentStep != null ? CurrentStep.Group : default!;

    public int CurrentStepNumber
    {
        get
        {
            var index = _configuration.OrderedSteps
                .ToList()
                .FindIndex(s => s.Id == CurrentStepId);
            return index >= 0 ? index + 1 : 0;
        }
    }

    public int TotalSteps => _configuration.OrderedSteps.Count;

    public bool CanGoBack => _stepHistory.Count > 0;

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        if (_persistence == null)
        {
            return;
        }

        var state = await _persistence.LoadStateAsync(ct);
        if (state == null)
        {
            return;
        }

        IsTutorialCompleted = state.IsCompleted;

        if (!state.IsCompleted && !state.IsSkipped && state.IsActive)
        {
            // Restore step statuses
            foreach (var (stepId, status) in state.StepStatuses)
            {
                _stepStatuses[stepId] = status;
            }

            // Find the group of the restored steps and restart from the group start
            if (_configuration.Steps.TryGetValue(state.CurrentStepId, out var step))
            {
                RestartGroupInternal(step.Group, skipPersist: true);
            }

            IsTutorialActive = true;
        }
    }

    public void StartTutorial()
    {
        _stepHistory.Clear();
        InitializeStepStatuses();
        CurrentStepId = _configuration.StartStepId;
        IsTutorialActive = true;
        IsTutorialCompleted = false;

        MarkCurrentStepVisited();
        PersistStateAsync().ConfigureAwait(false);
    }

    public void AdvanceStep()
    {
        if (!IsTutorialActive)
            return;

        var nextStepId = DetermineNextStep(CurrentStepId);
        if (nextStepId == null)
        {
            CompleteTutorial();
            return;
        }

        MarkCurrentStepCompleted();
        _stepHistory.Push(CurrentStepId);
        CurrentStepId = nextStepId;
        MarkCurrentStepVisited();

        OnPropertyChanged(nameof(CanGoBack));
        PersistCurrentStepAsync().ConfigureAwait(false);
    }

    public void GoBack()
    {
        if (!IsTutorialActive || _stepHistory.Count == 0)
            return;

        CurrentStepId = _stepHistory.Pop();
        OnPropertyChanged(nameof(CanGoBack));
        PersistCurrentStepAsync().ConfigureAwait(false);
    }

    public void SkipTutorial()
    {
        IsTutorialActive = false;
        PersistSkippedAsync().ConfigureAwait(false);
    }

    public async Task RestartTutorialAsync(CancellationToken ct = default)
    {
        _stepHistory.Clear();
        InitializeStepStatuses();
        CurrentStepId = _configuration.StartStepId;
        IsTutorialActive = true;
        IsTutorialCompleted = false;

        MarkCurrentStepVisited();

        if (_persistence != null)
        {
            await _persistence.SaveStateAsync(CreateState(), ct);
        }
    }

    public void FireTrigger(TTrigger trigger, Func<TContext, TContext>? contextUpdate = null)
    {
        if (!IsTutorialActive)
            return;

        var previousContext = Context;

        if (contextUpdate != null)
        {
            Context = contextUpdate(Context);
        }

        CheckForAutoAdvance(trigger, previousContext, Context);
    }

    public void UpdateContext(Func<TContext, TContext> update)
    {
        Context = update(Context);
    }

    public void RestartGroup(TGroup group)
    {
        RestartGroupInternal(group, skipPersist: false);
    }

    public StepStatus GetStepStatus(string stepId)
    {
        return _stepStatuses.GetValueOrDefault(stepId, StepStatus.NotVisited);
    }

    private void RestartGroupInternal(TGroup group, bool skipPersist)
    {
        if (!IsTutorialActive)
            return;

        // Reset all steps in a group to NotVisited
        var stepsInGroup = _configuration.GetStepsInGroup(group).ToList();
        foreach (var step in stepsInGroup)
        {
            _stepStatuses[step.Id] = StepStatus.NotVisited;
        }

        // Move to the first step of a group
        var firstStep = stepsInGroup.FirstOrDefault();
        if (firstStep == null)
            return;

        _stepHistory.Clear();
        CurrentStepId = firstStep.Id;
        MarkCurrentStepVisited();

        // Auto-advance through completed steps
        AdvanceWhileComplete();

        OnPropertyChanged(nameof(CanGoBack));
        if (!skipPersist)
        {
            PersistStateAsync().ConfigureAwait(false);
        }
    }

    private void AdvanceWhileComplete()
    {
        while (CurrentStep != null)
        {
            // Check if the current step's goal is already achieved
            if (CurrentStep.StepInstance is ITutorialStepComplete<TContext> completable
                && completable.IsComplete(Context))
            {
                var nextStepId = DetermineNextStep(CurrentStepId);
                if (nextStepId == null || nextStepId == CurrentStepId)
                    break;

                MarkCurrentStepCompleted();
                _stepHistory.Push(CurrentStepId);
                CurrentStepId = nextStepId;
                MarkCurrentStepVisited();
                continue;
            }

            break;
        }
    }

    private void CheckForAutoAdvance(TTrigger trigger, TContext previousContext, TContext currentContext)
    {
        var step = CurrentStep;
        if (step == null)
            return;

        // Check if the trigger is in the step's auto-advance triggers
        if (!step.AutoAdvanceTriggers.Contains(trigger))
            return;

        // If a step implements conditional auto-advance, check the condition
        if (step.StepInstance is IConditionalAutoAdvance<TContext, TTrigger> conditional)
        {
            if (!conditional.ShouldAutoAdvance(trigger, previousContext, currentContext))
                return;
        }

        AdvanceStep();
    }

    private string? DetermineNextStep(string currentStepId)
    {
        // Check configured transitions first (in order - first matching condition wins)
        var transitions = _configuration.Transitions
            .Where(t => t.FromStepId == currentStepId)
            .ToList();

        foreach (var transition in transitions)
        {
            if (transition.Condition == null || transition.Condition(Context))
            {
                return transition.ToStepId;
            }
        }

        // Fallback: find the next step in an ordered list
        var currentIndex = _configuration.OrderedSteps
            .ToList()
            .FindIndex(s => s.Id == currentStepId);

        if (currentIndex >= 0 && currentIndex < _configuration.OrderedSteps.Count - 1)
        {
            return _configuration.OrderedSteps[currentIndex + 1].Id;
        }

        return null; // No next step - tutorial complete
    }

    private void InitializeStepStatuses()
    {
        _stepStatuses.Clear();
        foreach (var step in _configuration.Steps.Values)
        {
            _stepStatuses[step.Id] = StepStatus.NotVisited;
        }
    }

    private void MarkCurrentStepVisited()
    {
        if (!string.IsNullOrEmpty(CurrentStepId))
        {
            _stepStatuses[CurrentStepId] = StepStatus.Visited;
        }
    }

    private void MarkCurrentStepCompleted()
    {
        if (!string.IsNullOrEmpty(CurrentStepId))
        {
            _stepStatuses[CurrentStepId] = StepStatus.Completed;
        }
    }

    private void CompleteTutorial()
    {
        MarkCurrentStepCompleted();
        IsTutorialCompleted = true;
        PersistCompletedAsync().ConfigureAwait(false);
    }

    private TutorialState CreateState()
    {
        return new TutorialState(
            IsActive: IsTutorialActive,
            CurrentStepId: CurrentStepId,
            IsCompleted: IsTutorialCompleted,
            IsSkipped: false,
            StepStatuses: new Dictionary<string, StepStatus>(_stepStatuses)
        );
    }

    private async Task PersistStateAsync()
    {
        if (_persistence == null)
            return;

        await _persistence.SaveStateAsync(CreateState());
    }

    private async Task PersistCurrentStepAsync()
    {
        if (_persistence == null)
            return;

        await _persistence.SaveStateAsync(CreateState());
    }

    private async Task PersistCompletedAsync()
    {
        if (_persistence == null)
            return;

        await _persistence.SaveStateAsync(new TutorialState(
            IsActive: false,
            CurrentStepId: CurrentStepId,
            IsCompleted: true,
            IsSkipped: false,
            StepStatuses: new Dictionary<string, StepStatus>(_stepStatuses)
        ));
    }

    private async Task PersistSkippedAsync()
    {
        if (_persistence == null)
            return;

        await _persistence.SaveStateAsync(new TutorialState(
            IsActive: false,
            CurrentStepId: CurrentStepId,
            IsCompleted: false,
            IsSkipped: true,
            StepStatuses: new Dictionary<string, StepStatus>(_stepStatuses)
        ));
    }
}
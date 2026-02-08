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

    [ObservableProperty]
    private TContext _context;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentStep))]
    [NotifyPropertyChangedFor(nameof(CurrentStepNumber))]
    [NotifyPropertyChangedFor(nameof(CurrentGroup))]
    [NotifyPropertyChangedFor(nameof(IsCurrentStepSkippable))]
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

    public bool IsCurrentStepSkippable
    {
        get
        {
            var step = CurrentStep;
            if (step == null)
                return false;

            if (!_configuration.SkipTransitions.ContainsKey(CurrentStepId))
                return false;

            return step.StepInstance is ITutorialStepSkippable<TContext> skippable
                   && skippable.CanSkip(Context);
        }
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        if (_persistence == null)
        {
            if (_configuration.AutoStartOnFirstRun)
            {
                StartTutorial();
            }

            return;
        }

        var state = await _persistence.LoadStateAsync(ct);
        if (state == null)
        {
            if (_configuration.AutoStartOnFirstRun)
            {
                StartTutorial();
            }

            return;
        }

        IsTutorialCompleted = state.IsCompleted;

        if (!state.IsCompleted && !state.IsSkipped && _configuration.AutoStartOnFirstRun)
        {
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        _stepHistory.Clear();
        CurrentStepId = _configuration.StartStepId;
        IsTutorialActive = true;
        IsTutorialCompleted = false;

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

        _stepHistory.Push(CurrentStepId);
        CurrentStepId = nextStepId;

        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(IsCurrentStepSkippable));
        PersistStateAsync().ConfigureAwait(false);
    }

    public void GoBack()
    {
        if (!IsTutorialActive || _stepHistory.Count == 0)
            return;

        CurrentStepId = _stepHistory.Pop();
        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(IsCurrentStepSkippable));
        PersistStateAsync().ConfigureAwait(false);
    }

    public void SkipTutorial()
    {
        IsTutorialActive = false;
        PersistSkippedAsync().ConfigureAwait(false);
    }

    public async Task RestartTutorialAsync(CancellationToken ct = default)
    {
        _stepHistory.Clear();
        CurrentStepId = _configuration.StartStepId;
        IsTutorialActive = true;
        IsTutorialCompleted = false;

        if (_persistence != null)
        {
            await _persistence.SaveStateAsync(CreateState(), ct);
        }
    }

    public void FireTrigger(TTrigger trigger, Func<TContext, TContext>? contextUpdate = null)
    {
        var previousContext = Context;

        if (contextUpdate != null)
        {
            Context = contextUpdate(Context);
        }

        if (!IsTutorialActive)
        {
            return;
        }

        CheckForAutoAdvance(trigger, previousContext, Context);
    }

    public void UpdateContext(Func<TContext, TContext> update)
    {
        Context = update(Context);
        OnPropertyChanged(nameof(IsCurrentStepSkippable));
    }

    public void RestartGroup(TGroup group)
    {
        RestartGroupInternal(group);
    }

    public void SkipStep()
    {
        if (!IsTutorialActive || !IsCurrentStepSkippable)
        {
            return;
        }

        if (!_configuration.SkipTransitions.TryGetValue(CurrentStepId, out var targetStepId))
        {
            return;
        }

        _stepHistory.Push(CurrentStepId);
        CurrentStepId = targetStepId;

        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(IsCurrentStepSkippable));
        PersistStateAsync().ConfigureAwait(false);
    }

    public async Task RestartFromCurrentViewAsync(CancellationToken ct = default)
    {
        // Reactivate tutorial (may have been completed/skipped)
        IsTutorialActive = true;
        IsTutorialCompleted = false;

        foreach (var (group, condition) in _configuration.GroupViewMappings)
        {
            if (!condition(Context))
            {
                continue;
            }

            RestartGroupInternal(group);

            if (_persistence != null)
            {
                await _persistence.SaveStateAsync(CreateState(), ct);
            }

            return;
        }

        await RestartTutorialAsync(ct);
    }

    private void RestartGroupInternal(TGroup group)
    {
        if (!IsTutorialActive)
            return;

        var firstStep = _configuration.GetFirstStepInGroup(group);
        if (firstStep == null)
            return;

        _stepHistory.Clear();
        CurrentStepId = firstStep.Id;

        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(IsCurrentStepSkippable));
        PersistStateAsync().ConfigureAwait(false);
    }

    private void CheckForAutoAdvance(TTrigger trigger, TContext previousContext, TContext currentContext)
    {
        var step = CurrentStep;
        if (step == null)
        {
            return;
        }

        if (!step.AutoAdvanceTriggers.Contains(trigger))
        {
            return;
        }

        if (step.StepInstance is IConditionalAutoAdvance<TContext, TTrigger> conditional)
        {
            if (!conditional.ShouldAutoAdvance(trigger, previousContext, currentContext))
            {
                return;
            }
        }

        AdvanceStep();
    }

    private string? DetermineNextStep(string currentStepId)
    {
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

        var currentIndex = _configuration.OrderedSteps
            .ToList()
            .FindIndex(s => s.Id == currentStepId);

        if (currentIndex >= 0 && currentIndex < _configuration.OrderedSteps.Count - 1)
        {
            return _configuration.OrderedSteps[currentIndex + 1].Id;
        }

        return null;
    }

    private void CompleteTutorial()
    {
        IsTutorialCompleted = true;
        IsTutorialActive = false;
        PersistCompletedAsync().ConfigureAwait(false);
    }

    private TutorialState CreateState()
    {
        return new TutorialState(
            IsCompleted: IsTutorialCompleted,
            IsSkipped: false
        );
    }

    private async Task PersistStateAsync()
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
            IsCompleted: true,
            IsSkipped: false
        ));
    }

    private async Task PersistSkippedAsync()
    {
        if (_persistence == null)
            return;

        await _persistence.SaveStateAsync(new TutorialState(
            IsCompleted: false,
            IsSkipped: true
        ));
    }
}
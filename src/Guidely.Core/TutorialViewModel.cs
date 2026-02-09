using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Guidely.Core.Abstractions;

namespace Guidely.Core;

/// <summary>
/// Base ViewModel for the tutorial overlay UI, providing bindable properties and commands.
/// </summary>
/// <typeparam name="TContext">The tutorial context type.</typeparam>
/// <typeparam name="TTrigger">The trigger enum type.</typeparam>
/// <typeparam name="TGroup">The group enum type.</typeparam>
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public partial class TutorialViewModel<TContext, TTrigger, TGroup> : ObservableObject, IDisposable
    where TContext : TutorialContextBase, new()
    where TTrigger : Enum
    where TGroup : Enum
{
    private readonly ITutorialService<TContext, TTrigger, TGroup> _tutorialService;

    [ObservableProperty]
    private string _actionHintText = "Perform the action to continue";

    [ObservableProperty]
    private bool _canGoBack;

    [ObservableProperty]
    private ClickThroughMode _clickThroughMode = ClickThroughMode.None;

    [ObservableProperty]
    private string _currentStepId = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private bool _isTutorialCompleted;

    [ObservableProperty]
    private bool _isVisible;

    [ObservableProperty]
    private string _nextButtonText = "Next";

    [ObservableProperty]
    private bool _requiresUserAction;

    [ObservableProperty]
    private bool _showNextButton = true;

    [ObservableProperty]
    private string _stepProgress = string.Empty;

    [ObservableProperty]
    private IReadOnlyList<string> _targets = Array.Empty<string>();

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private TooltipPosition _tooltipPosition;

    public TutorialViewModel(ITutorialService<TContext, TTrigger, TGroup> tutorialService)
    {
        _tutorialService = tutorialService;
        _tutorialService.PropertyChanged += OnServicePropertyChanged;
    }

    /// <summary>
    /// The underlying tutorial service.
    /// </summary>
    public ITutorialService<TContext, TTrigger, TGroup> Service => _tutorialService;

    public void Dispose()
    {
        _tutorialService.PropertyChanged -= OnServicePropertyChanged;
    }

    /// <summary>
    /// Initializes the tutorial view model and underlying service.
    /// </summary>
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await _tutorialService.InitializeAsync(ct);
        UpdateFromCurrentStep();
    }

    /// <summary>
    /// Starts the tutorial.
    /// </summary>
    public void StartTutorial()
    {
        _tutorialService.StartTutorial();
        UpdateFromCurrentStep();
    }

    /// <summary>
    /// Fires a trigger with an optional context update.
    /// </summary>
    public void FireTrigger(TTrigger trigger, Func<TContext, TContext>? contextUpdate = null)
    {
        _tutorialService.FireTrigger(trigger, contextUpdate);
    }

    /// <summary>
    /// Updates the context without firing a trigger.
    /// </summary>
    public void UpdateContext(Func<TContext, TContext> update)
    {
        _tutorialService.UpdateContext(update);
    }

    /// <summary>
    /// Restarts the tutorial from the beginning of the specified group.
    /// </summary>
    public void RestartGroup(TGroup group)
    {
        _tutorialService.RestartGroup(group);
    }

    [RelayCommand]
    private void OnNext()
    {
        if (_tutorialService.IsTutorialCompleted)
        {
            IsVisible = false;
            return;
        }

        if (_tutorialService.IsCurrentStepSkippable)
        {
            _tutorialService.SkipStep();
        }
        else
        {
            _tutorialService.AdvanceStep();
        }
    }

    [RelayCommand]
    private void OnBack()
    {
        _tutorialService.GoBack();
    }

    [RelayCommand]
    private void OnSkip()
    {
        _tutorialService.SkipTutorial();
        IsVisible = false;
    }

    [RelayCommand]
    private async Task OnRestartAsync(CancellationToken ct)
    {
        await _tutorialService.RestartFromCurrentViewAsync(ct);
    }

    [RelayCommand]
    private void OnDismiss()
    {
        IsVisible = false;
    }

    private void OnServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ITutorialService<TContext, TTrigger, TGroup>.CurrentStep):
            case nameof(ITutorialService<TContext, TTrigger, TGroup>.IsTutorialActive):
            case nameof(ITutorialService<TContext, TTrigger, TGroup>.IsTutorialCompleted):
            case nameof(ITutorialService<TContext, TTrigger, TGroup>.CurrentStepId):
            case nameof(ITutorialService<TContext, TTrigger, TGroup>.IsCurrentStepSkippable):
            case nameof(ITutorialService<TContext, TTrigger, TGroup>.CanGoBack):
                UpdateFromCurrentStep();
                break;
        }
    }

    /// <summary>
    /// Updates all view model properties from the current step.
    /// Override in derived classes to add custom update logic.
    /// </summary>
    protected virtual void UpdateFromCurrentStep()
    {
        var step = _tutorialService.CurrentStep;
        if (step is null)
        {
            IsVisible = false;
            return;
        }

        IsVisible = _tutorialService.IsTutorialActive;
        IsTutorialCompleted = _tutorialService.IsTutorialCompleted;
        CurrentStepId = _tutorialService.CurrentStepId;

        // Use skip content when step is skippable
        if (_tutorialService.IsCurrentStepSkippable
            && step.StepInstance is ITutorialStepSkippable<TContext> skippable)
        {
            Title = skippable.SkipTitle;
            Description = skippable.SkipDescription;
        }
        else
        {
            Title = step.Title;
            Description = step.Description;
        }

        Targets = step.Targets;
        TooltipPosition = step.Position;
        CanGoBack = _tutorialService.CanGoBack;
        ActionHintText = GetActionHintText(step.Position);

        // Update progress
        var currentNumber = _tutorialService.CurrentStepNumber;
        var total = _tutorialService.TotalSteps;
        StepProgress = $"Step {currentNumber} of {total}";

        // Update button text and action state based on the step
        RequiresUserAction = step.RequiresUserAction;
        ClickThroughMode = step.ClickThroughMode;

        // Determine button visibility based on step configuration
        UpdateButtonState(step);
    }

    private static string GetActionHintText(TooltipPosition position) => position switch
    {
        TooltipPosition.Top => "Perform the action below to continue",
        TooltipPosition.Bottom => "Perform the action above to continue",
        TooltipPosition.Left => "Perform the action to the right to continue",
        TooltipPosition.Right => "Perform the action to the left to continue",
        _ => "Perform the highlighted action to continue"
    };

    /// <summary>
    /// Updates button text and visibility based on the current step.
    /// Override in derived classes to customize button behavior.
    /// </summary>
    protected virtual void UpdateButtonState(TutorialStepMetadata<TTrigger, TGroup> step)
    {
        if (_tutorialService.IsTutorialCompleted ||
            _tutorialService.CurrentStepNumber == _tutorialService.TotalSteps)
        {
            NextButtonText = "Finish";
            ShowNextButton = true;
        }
        else if (_tutorialService.IsCurrentStepSkippable)
        {
            // Step can be skipped — show Next even if RequiresUserAction
            NextButtonText = "Next";
            ShowNextButton = true;
        }
        else if (step.RequiresUserAction)
        {
            // For steps that require user action, hide the next button
            // The step will auto-advance when the action is performed
            ShowNextButton = false;
        }
        else
        {
            NextButtonText = "Next";
            ShowNextButton = true;
        }
    }
}
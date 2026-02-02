using AvailabilityCompass.Core.Features.Tutorial.Events;
using AvailabilityCompass.Core.Shared.EventBus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// ViewModel for the tutorial overlay UI, providing bindable properties and commands.
/// </summary>
public sealed partial class TutorialViewModel : ObservableObject, IDisposable
{
    private readonly IEventBus _eventBus;
    private readonly List<IDisposable> _subscriptions = [];
    private readonly ITutorialService _tutorialService;

    [ObservableProperty]
    private bool _canGoBack;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private bool _isVisible;

    [ObservableProperty]
    private string _nextButtonText = "Next";

    [ObservableProperty]
    private TutorialTargetElement _primaryTarget;

    [ObservableProperty]
    private TutorialTargetElement? _secondaryTarget;

    [ObservableProperty]
    private bool _showNextButton = true;

    [ObservableProperty]
    private bool _requiresUserAction;

    [ObservableProperty]
    private ClickThroughMode _clickThroughMode = ClickThroughMode.NoneClickable;

    [ObservableProperty]
    private string _stepProgress = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private TooltipPosition _tooltipPosition;

    public TutorialViewModel(ITutorialService tutorialService, IEventBus eventBus)
    {
        _tutorialService = tutorialService;
        _eventBus = eventBus;

        SubscribeToEvents();
    }

    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }

        _subscriptions.Clear();
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await _tutorialService.InitializeAsync(ct);
        UpdateFromCurrentStep();
    }

    [RelayCommand]
    private void OnNext()
    {
        _tutorialService.AdvanceStep();
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
        await _tutorialService.RestartTutorialAsync(ct);
    }

    [RelayCommand]
    private void OnDismiss()
    {
        IsVisible = false;
    }

    private void SubscribeToEvents()
    {
        _subscriptions.Add(
            _eventBus.Listen<TutorialStepChangedEvent>()
                .Subscribe(_ => UpdateFromCurrentStep())
        );

        _subscriptions.Add(
            _eventBus.Listen<TutorialStartedEvent>()
                .Subscribe(_ => UpdateFromCurrentStep())
        );

        _subscriptions.Add(
            _eventBus.Listen<TutorialSkippedEvent>()
                .Subscribe(_ => IsVisible = false)
        );

        _subscriptions.Add(
            _eventBus.Listen<TutorialCompletedEvent>()
                .Subscribe(_ => UpdateFromCurrentStep())
        );
    }

    private void UpdateFromCurrentStep()
    {
        var step = _tutorialService.CurrentStep;
        if (step is null)
        {
            IsVisible = false;
            return;
        }

        IsVisible = _tutorialService.IsTutorialActive;
        Title = step.Title;
        Description = step.Description;
        PrimaryTarget = step.PrimaryTarget;
        SecondaryTarget = step.SecondaryTarget;
        TooltipPosition = step.Position;
        CanGoBack = _tutorialService.CurrentStepId != TutorialStepId.Welcome;

        // Update progress
        var currentNumber = _tutorialService.CurrentStepNumber;
        var total = _tutorialService.TotalSteps;
        StepProgress = $"Step {currentNumber} of {total}";

        // Update button text and action state based on the step
        RequiresUserAction = step.RequiresUserAction;

        // Determine click-through mode: use explicit value or auto-determine from RequiresUserAction
        ClickThroughMode = step.ClickThroughMode ??
            (step.RequiresUserAction ? ClickThroughMode.OnlyHighlightedClickable : ClickThroughMode.NoneClickable);

        if (_tutorialService.CurrentStepId == TutorialStepId.TutorialComplete)
        {
            NextButtonText = "Finish";
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

    /// <summary>
    /// Directly sets the tutorial context.
    /// </summary>
    public void SetContext(TutorialContext context)
    {
        _tutorialService.UpdateContext(context);
    }
}
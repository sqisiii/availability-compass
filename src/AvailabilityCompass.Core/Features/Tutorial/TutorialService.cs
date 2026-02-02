using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddDateEntryRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Events;
using AvailabilityCompass.Core.Features.ManageSettings.Commands.SaveSetting;
using AvailabilityCompass.Core.Features.ManageSettings.Queries.GetSetting;
using AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;
using AvailabilityCompass.Core.Features.SearchRecords.Events;
using AvailabilityCompass.Core.Features.Tutorial.Events;
using AvailabilityCompass.Core.Shared.EventBus;
using CommunityToolkit.Mvvm.ComponentModel;
using MediatR;

namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// State machine managing the tutorial flow, persistence, and event handling.
/// </summary>
public sealed partial class TutorialService : ObservableObject, ITutorialService, IDisposable
{
    private const string TutorialActiveKey = "Tutorial_Active";
    private const string TutorialStepKey = "Tutorial_CurrentStep";
    private const string TutorialCompletedKey = "Tutorial_Completed";
    private const string TutorialSkippedKey = "Tutorial_Skipped";
    private readonly IEventBus _eventBus;

    private readonly IMediator _mediator;
    private readonly Stack<TutorialStepId> _stepHistory = new();
    private readonly List<IDisposable> _subscriptions = [];

    [ObservableProperty]
    private TutorialContext _context = TutorialContext.Default;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentStep))]
    [NotifyPropertyChangedFor(nameof(CurrentStepNumber))]
    private TutorialStepId _currentStepId = TutorialStepId.Welcome;

    [ObservableProperty]
    private bool _isTutorialActive;

    [ObservableProperty]
    private bool _isTutorialCompleted;

    public TutorialService(IMediator mediator, IEventBus eventBus)
    {
        _mediator = mediator;
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

    public TutorialStepDefinition? CurrentStep => IsTutorialActive ? TutorialStepRegistry.GetStep(CurrentStepId) : null;

    public int CurrentStepNumber => TutorialStepRegistry.GetStepIndex(CurrentStepId) + 1;

    public int TotalSteps => TutorialStepRegistry.TotalStepsForProgress;

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        var completedResponse = await _mediator.Send(new GetSettingQuery(TutorialCompletedKey, "false"), ct);
        var skippedResponse = await _mediator.Send(new GetSettingQuery(TutorialSkippedKey, "false"), ct);
        var activeResponse = await _mediator.Send(new GetSettingQuery(TutorialActiveKey, "true"), ct);

        IsTutorialCompleted = bool.Parse(completedResponse.Value ?? "false");
        var wasSkipped = bool.Parse(skippedResponse.Value ?? "false");
        var wasActive = bool.Parse(activeResponse.Value ?? "true");

        if (!IsTutorialCompleted && !wasSkipped && wasActive)
        {
            var stepResponse = await _mediator.Send(new GetSettingQuery(TutorialStepKey, nameof(TutorialStepId.Welcome)), ct);
            if (Enum.TryParse<TutorialStepId>(stepResponse.Value, out var stepId))
            {
                CurrentStepId = stepId;
            }

            IsTutorialActive = true;
        }
    }

    public void StartTutorial()
    {
        _stepHistory.Clear();
        CurrentStepId = TutorialStepId.Welcome;
        IsTutorialActive = true;
        IsTutorialCompleted = false;

        PersistStateAsync().ConfigureAwait(false);
        _eventBus.Publish(new TutorialStartedEvent());
        _eventBus.Publish(new TutorialStepChangedEvent());
    }

    public void AdvanceStep()
    {
        if (!IsTutorialActive)
            return;

        var previousStep = CurrentStepId;
        var nextStep = DetermineNextStep(CurrentStepId);

        if (nextStep == TutorialStepId.TutorialComplete)
        {
            CompleteTutorial();
            return;
        }

        _stepHistory.Push(previousStep);
        CurrentStepId = nextStep;

        PersistCurrentStepAsync().ConfigureAwait(false);
        _eventBus.Publish(new TutorialStepChangedEvent());
    }

    public void GoBack()
    {
        if (!IsTutorialActive || _stepHistory.Count == 0)
            return;

        CurrentStepId = _stepHistory.Pop();

        PersistCurrentStepAsync().ConfigureAwait(false);
        _eventBus.Publish(new TutorialStepChangedEvent());
    }

    public void SkipTutorial()
    {
        IsTutorialActive = false;
        PersistSkippedAsync().ConfigureAwait(false);
        _eventBus.Publish(new TutorialSkippedEvent());
    }

    public async Task RestartTutorialAsync(CancellationToken ct = default)
    {
        await _mediator.Send(new SaveSettingRequest(TutorialCompletedKey, "false"), ct);
        await _mediator.Send(new SaveSettingRequest(TutorialSkippedKey, "false"), ct);
        await _mediator.Send(new SaveSettingRequest(TutorialActiveKey, "true"), ct);
        await _mediator.Send(new SaveSettingRequest(TutorialStepKey, nameof(TutorialStepId.Welcome)), ct);

        StartTutorial();
    }

    public void UpdateContext(TutorialContext context)
    {
        var previousContext = Context;
        Context = context;

        _eventBus.Publish(new TutorialContextChangedEvent());

        // Check if context change should auto-advance the current step
        if (IsTutorialActive)
        {
            CheckForAutoAdvance(previousContext, context);
        }
    }

    private TutorialStepId DetermineNextStep(TutorialStepId currentStep)
    {
        return currentStep switch
        {
            TutorialStepId.Welcome => DetermineStepAfterWelcome(),
            TutorialStepId.SourcesDialogRefreshButtons => Context.HasRefreshedSource
                ? TutorialStepId.PointToCalendarsButton
                : TutorialStepId.WaitForSourceRefresh,
            TutorialStepId.PointToSourcesButton => TutorialStepId.SourcesDialogRefreshButtons,
            TutorialStepId.PointToCalendarsButton => TutorialStepId.CalendarsOverview,
            TutorialStepId.WaitForSourceRefresh => TutorialStepId.SourcesDialogRefreshButtons,
            TutorialStepId.CalendarsOverview => DetermineStepAfterCalendarsOverview(),
            TutorialStepId.ClickExistingCalendar => TutorialStepId.CalendarViewOverview,
            TutorialStepId.ClickAddCalendarButton => TutorialStepId.AddCalendarFormExplanation,
            TutorialStepId.AddCalendarFormExplanation => TutorialStepId.CalendarViewOverview,
            TutorialStepId.CalendarViewOverview => DetermineStepAfterCalendarView(),
            TutorialStepId.AddDaysExplanation => TutorialStepId.DateEntriesExplanation,
            TutorialStepId.DateEntriesExplanation => TutorialStepId.PointToSearchView,
            TutorialStepId.PointToSearchView => TutorialStepId.CalendarFilterExplanation,
            TutorialStepId.CalendarFilterExplanation => TutorialStepId.SourcesFilterExplanation,
            TutorialStepId.SourcesFilterExplanation => Context.HasSourceFilterSelected
                ? TutorialStepId.SourceFilterOptionsExplanation
                : TutorialStepId.FiltersExplanation,
            TutorialStepId.SourceFilterOptionsExplanation => TutorialStepId.FiltersExplanation,
            TutorialStepId.FiltersExplanation => TutorialStepId.SearchButtonExplanation,
            TutorialStepId.SearchButtonExplanation => TutorialStepId.ResultsExplanation,
            _ => TutorialStepId.TutorialComplete
        };
    }

    private TutorialStepId DetermineStepAfterWelcome()
    {
        // If the sources dialog is already open, show refresh buttons
        if (Context.CurrentDialog == DialogType.Sources)
            return TutorialStepId.SourcesDialogRefreshButtons;

        // Otherwise point to the sources button
        return TutorialStepId.PointToSourcesButton;
    }

    private TutorialStepId DetermineStepAfterCalendarsOverview()
    {
        return Context.HasCalendars
            ? TutorialStepId.ClickExistingCalendar
            : TutorialStepId.ClickAddCalendarButton;
    }

    private TutorialStepId DetermineStepAfterCalendarView()
    {
        return Context.HasCalendarEntries
            ? TutorialStepId.DateEntriesExplanation
            : TutorialStepId.AddDaysExplanation;
    }

    private void CheckForAutoAdvance(TutorialContext previousContext, TutorialContext newContext)
    {
        // Auto-advance based on context changes
        switch (CurrentStepId)
        {
            case TutorialStepId.PointToSourcesButton when newContext.CurrentDialog == DialogType.Sources:
                AdvanceStep();
                break;

            case TutorialStepId.SourcesDialogRefreshButtons when !previousContext.HasRefreshedSource && newContext.HasRefreshedSource:
                AdvanceStep();
                break;

            case TutorialStepId.WaitForSourceRefresh when newContext.HasRefreshedSource:
                CurrentStepId = TutorialStepId.PointToCalendarsButton;
                PersistCurrentStepAsync().ConfigureAwait(false);
                _eventBus.Publish(new TutorialStepChangedEvent());
                break;

            case TutorialStepId.PointToCalendarsButton when newContext.CurrentDialog == DialogType.Calendars:
                AdvanceStep();
                break;

            case TutorialStepId.ClickAddCalendarButton when newContext.IsAddCalendarExpanded:
                AdvanceStep();
                break;

            case TutorialStepId.AddCalendarFormExplanation when !previousContext.HasCalendars && newContext.HasCalendars:
                AdvanceStep();
                break;

            case TutorialStepId.ClickExistingCalendar when newContext.IsCalendarSelected:
                AdvanceStep();
                break;

            case TutorialStepId.AddDaysExplanation when newContext.IsEditorOpen:
                // Don't auto-advance here - wait for entry to be added
                break;

            case TutorialStepId.AddDaysExplanation when !previousContext.HasCalendarEntries && newContext.HasCalendarEntries:
                AdvanceStep();
                break;

            case TutorialStepId.PointToSearchView when newContext.CurrentDialog == DialogType.None:
                AdvanceStep();
                break;

            case TutorialStepId.CalendarFilterExplanation when newContext.HasCalendarFilterSelected:
                AdvanceStep();
                break;

            case TutorialStepId.SourcesFilterExplanation when newContext.HasSourceFilterSelected:
                AdvanceStep();
                break;

            case TutorialStepId.SearchButtonExplanation when !previousContext.HasSearchResults && newContext.HasSearchResults:
                AdvanceStep();
                break;
        }
    }

    private void CompleteTutorial()
    {
        CurrentStepId = TutorialStepId.TutorialComplete;
        IsTutorialCompleted = true;

        // Show completion step briefly, then deactivate
        PersistCompletedAsync().ConfigureAwait(false);
        _eventBus.Publish(new TutorialStepChangedEvent());
        _eventBus.Publish(new TutorialCompletedEvent());
    }

    private void SubscribeToEvents()
    {
        // Subscribe to source refresh events
        _subscriptions.Add(
            _eventBus.Listen<SourcesDataChangedEvent>()
                .Subscribe(_ => OnSourceDataChanged())
        );

        // Subscribe to calendar events
        _subscriptions.Add(
            _eventBus.Listen<CalendarAddedEvent>()
                .Subscribe(_ => OnCalendarAdded())
        );

        _subscriptions.Add(
            _eventBus.Listen<AddCalendarFormExpandedEvent>()
                .Subscribe(_ => OnAddCalendarFormExpanded())
        );

        _subscriptions.Add(
            _eventBus.Listen<DateEntryAddedEvent>()
                .Subscribe(_ => OnDateEntryAdded())
        );

        // Subscribe to search events
        _subscriptions.Add(
            _eventBus.Listen<SearchResultsFoundEvent>()
                .Subscribe(_ => OnSearchResultsFound())
        );
    }

    private void OnSourceDataChanged()
    {
        if (!IsTutorialActive)
            return;

        UpdateContext(Context with { HasRefreshedSource = true, HasSourcesWithData = true });
    }

    private void OnCalendarAdded()
    {
        if (!IsTutorialActive)
            return;

        UpdateContext(Context with { HasCalendars = true, IsAddCalendarExpanded = false });
    }

    private void OnAddCalendarFormExpanded()
    {
        if (!IsTutorialActive)
            return;

        UpdateContext(Context with { IsAddCalendarExpanded = true });
    }

    private void OnDateEntryAdded()
    {
        if (!IsTutorialActive)
            return;

        UpdateContext(Context with { HasCalendarEntries = true, IsEditorOpen = false });
    }

    private void OnSearchResultsFound()
    {
        if (!IsTutorialActive)
            return;

        UpdateContext(Context with { HasSearchResults = true });
    }

    private async Task PersistStateAsync()
    {
        await _mediator.Send(new SaveSettingRequest(TutorialActiveKey, IsTutorialActive.ToString().ToLowerInvariant()));
        await _mediator.Send(new SaveSettingRequest(TutorialStepKey, CurrentStepId.ToString()));
    }

    private async Task PersistCurrentStepAsync()
    {
        await _mediator.Send(new SaveSettingRequest(TutorialStepKey, CurrentStepId.ToString()));
    }

    private async Task PersistCompletedAsync()
    {
        await _mediator.Send(new SaveSettingRequest(TutorialCompletedKey, "true"));
        await _mediator.Send(new SaveSettingRequest(TutorialActiveKey, "false"));
    }

    private async Task PersistSkippedAsync()
    {
        await _mediator.Send(new SaveSettingRequest(TutorialSkippedKey, "true"));
        await _mediator.Send(new SaveSettingRequest(TutorialActiveKey, "false"));
    }
}
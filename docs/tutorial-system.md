# Tutorial System Architecture

## Overview

The tutorial system provides step-by-step onboarding that guides new users through the application's features using tooltips, highlights, and contextual descriptions. It uses a state machine pattern with data-driven step definitions for easy modification.

**Key Features:**

- Persists progress to database (survives app restarts)
- Conditional branching based on app state
- Auto-advances when user completes expected actions
- Restart anytime via header button

---

## Architecture Components

### Guidely Package

The tutorial system is built on the **Guidely** NuGet package which provides:

- `TutorialService<TContext, TTrigger, TGroup>` - State machine managing flow and persistence
- `TutorialViewModel<TContext, TTrigger, TGroup>` - UI binding layer with commands
- `ITutorialPersistence` - Interface for state persistence
- Step definition attributes: `[TutorialStep]`, `[TutorialTarget]`, `[AutoAdvanceOn]`
- Interfaces: `ITutorialStepContent`, `IConditionalAutoAdvance`, `ITutorialStepComplete`

### Application Layer (`AvailabilityCompass.Core/Features/Tutorial/`)

| Component | Purpose |
|-----------|---------|
| `TutorialStepIds.cs` | String constants for all step identifiers |
| `AppTutorialTrigger.cs` | Enum of application-specific triggers |
| `AppTutorialGroup.cs` | Enum of step groups (Introduction, Sources, Calendars, Search, Complete) |
| `AvailabilityCompassContext.cs` | App state record for conditional branching |
| `TutorialSetup.cs` | Transition configuration between steps |
| `Steps/*.cs` | Individual step definitions with content and behavior |

### WPF Layer (`Guidely.WPF/` + `AvailabilityCompass.WpfClient/`)

| Component | Purpose |
|-----------|---------|
| `TutorialOverlay.xaml` | Visual tooltip + highlight control |
| `TutorialElementRegistry.cs` | Tracks loaded UI elements by key |
| `TutorialTarget` attached property | Marks elements as tutorial targets |

---

## Component Interaction

### Startup Flow

```
MainViewModel.InitializeAsync()
    │
    ├── Load ViewModels (Search, Sources, Calendars)
    │
    ├── TutorialViewModel.InitializeAsync()
    │   └── TutorialService.InitializeAsync()
    │       └── Load persisted state from Settings
    │
    └── UpdateTutorialContext()
        └── Sets initial app state (HasSources, CurrentDialog, etc.)
```

### Step Progression

```
User clicks "Next"
    │
    ▼
TutorialService.AdvanceStep()
    │
    ▼
DetermineNextStep(currentStep)
    │
    ├── Linear progression (most steps)
    │
    ├── Conditional branching (based on TutorialContext)
    │   Example: CalendarsOverview → HasCalendars ? ClickExistingCalendar : ClickAddCalendarButton
    │
    └── Tutorial complete
```

### Auto-Advance Mechanism

```
EventBus publishes event (e.g., SourcesDataChangedEvent)
    │
    ▼
TutorialService subscriber receives event
    │
    ▼
UpdateContext(Context with { HasRefreshedSource = true })
    │
    ▼
CheckForAutoAdvance()
    │
    └── If conditions match current step → AdvanceStep()
```

### Element Targeting

```
XAML: <Button tutorial:TutorialTarget.Element="RefreshAllButton">
    │
    ▼ (Loaded event)
TutorialTarget.OnElementLoaded()
    │
    ▼
TutorialElementRegistry.Register(RefreshAllButton, element)
    │
    ▼
TutorialOverlay requests element for current step
    │
    ▼
TutorialElementRegistry.GetElement(RefreshAllButton)
    │
    ▼
TutorialHighlightAdorner draws pulsing border
```

---

## Tutorial Step Flow

```
Welcome
    │
    ├─[Sources dialog open]──► SourcesDialogRefreshButtons
    │                                   │
    │                                   └──► PointToCalendarsButton
    │
    └─[Sources dialog closed]──► PointToSourcesButton ──┘
                                        │
                                        ▼
                                CalendarsOverview
                                        │
                    ┌───────────────────┴───────────────────┐
                    │                                       │
            [Has calendars]                         [No calendars]
                    │                                       │
                    ▼                                       ▼
          ClickExistingCalendar                  ClickAddCalendarButton
                    │                                       │
                    │                                       ▼
                    │                          AddCalendarFormExplanation
                    │                                       │
                    └───────────────┬───────────────────────┘
                                    │
                                    ▼
                          CalendarViewOverview
                                    │
                    ┌───────────────┴───────────────┐
                    │                               │
            [Has entries]                    [No entries]
                    │                               │
                    ▼                               ▼
          DateEntriesExplanation ◄───── AddDaysExplanation
                    │
                    ▼
            PointToSearchView
                    │
                    ▼
         CalendarFilterExplanation
                    │
                    ▼
         SourcesFilterExplanation
                    │
                    ├─[Source selected]──► SourceFilterOptionsExplanation
                    │                               │
                    └───────────────────────────────┘
                                    │
                                    ▼
                          FiltersExplanation
                                    │
                                    ▼
                       SearchButtonExplanation
                                    │
                                    ▼
                          ResultsExplanation
                                    │
                                    ▼
                          TutorialComplete
```

---

## Tutorial Step Details

| # | Step ID | Group | Target Element | Trigger | Auto-Advances When | Context Updated |
|---|---------|-------|----------------|---------|-------------------|-----------------|
| 1 | Welcome | Introduction | None (centered) | - | Manual: Next button | - |
| 2 | PointToSourcesButton | Sources | SourcesHeaderButton | DialogChanged | Sources dialog opens | CurrentDialog = Sources |
| 3 | SourcesDialogRefreshButtons | Sources | RefreshAllButton, SourceCardRefreshButton | SourceRefreshed | Any source is refreshed | HasRefreshedSource = true |
| 4 | WaitForSourceRefresh | Sources | SourcesHeaderButton | SourceRefreshed | Refresh completes | HasSourcesWithData = true |
| 5 | PointToCalendarsButton | Calendars | CalendarsHeaderButton | DialogChanged | Calendars dialog opens | CurrentDialog = Calendars |
| 6 | CalendarsOverview | Calendars | None (centered) | - | Manual: Next button | - |
| 7a | ClickExistingCalendar | Calendars | CalendarSelector | CalendarSelected | A calendar is clicked | IsCalendarSelected = true, HasCalendarEntries |
| 7b | ClickAddCalendarButton | Calendars | AddCalendarButton | CalendarFormExpanded | Add form expands | IsAddCalendarExpanded = true |
| 8 | AddCalendarFormExplanation | Calendars | AddCalendarForm | CalendarAdded | Calendar is saved | HasCalendars = true |
| 9 | CalendarViewOverview | Calendars | CalendarWidget | - | Manual: Next button | - |
| 10 | AddDaysExplanation | Calendars | AddDaysButton | DateEntryAdded | Date entry is saved | HasCalendarEntries = true |
| 11 | DateEntriesExplanation | Calendars | DateEntriesPanel | - | Manual: Next button | - |
| 12 | PointToSearchView | Search | None (centered) | DialogChanged | Calendars dialog closes | CurrentDialog = None |
| 13 | CalendarFilterExplanation | Search | CalendarFilterSection | FilterSelected | Calendar filter toggled | HasCalendarFilterSelected = true |
| 14 | SourcesFilterExplanation | Search | SourcesFilterSection | FilterSelected | Source filter toggled | HasSourceFilterSelected = true |
| 15 | SourceFilterOptionsExplanation | Search | SourceFilterOptions | - | Manual: Next button | - |
| 16 | FiltersExplanation | Search | FiltersSection | - | Manual: Next button | - |
| 17 | SearchButtonExplanation | Search | SearchButton | SearchPerformed | Search returns results | HasSearchResults = true |
| 18 | ResultsExplanation | Search | ResultsSection | - | Manual: Next button | - |
| 19 | TutorialComplete | Complete | None (centered) | - | User clicks Finish | - |

**Notes:**

- Steps 7a/7b are mutually exclusive based on `HasCalendars` context
- Steps marked with "-" for Trigger require manual advancement via Next button
- Skippable steps check `IsComplete()` - if already satisfied, the step is auto-skipped

---

## Marked UI Elements

### MainWindow.xaml

- `SourcesHeaderButton` - Sources button in header
- `CalendarsHeaderButton` - Calendars button in header

### ManageSourcesView.xaml

- `RefreshAllButton` - Refresh All button
- `SourceCardRefreshButton` - Source card Refresh button

### ManageCalendarsView.xaml

- `CalendarSelector` - Calendar list
- `AddCalendarButton` - Add calendar (+) button
- `AddCalendarForm` - New calendar form
- `CalendarEditButton` - Edit button
- `CalendarDeleteButton` - Delete button
- `CalendarWidget` - Calendar date picker
- `AddDaysButton` - Add Dates button
- `DateEntriesPanel` - Date entries list panel

### SearchView.xaml

- `CalendarFilterSection` - Calendars filter header
- `SourcesFilterSection` - Sources filter header
- `SourceFilterOptions` - Source-specific filter options
- `FiltersSection` - Global filters header
- `SearchButton` - Search button
- `ResultsSection` - Search results area

---

## Adding New Steps

### 1. Add Step ID

```csharp
// TutorialStepId.cs
public enum TutorialStepId
{
    // ... existing steps
    NewStepName,
}
```

### 2. Add Target Element (if needed)

```csharp
// TutorialTargetElement.cs
public enum TutorialTargetElement
{
    // ... existing elements
    NewTargetElement,
}
```

### 3. Define Step

```csharp
// TutorialStepRegistry.cs
private static readonly Dictionary<TutorialStepId, TutorialStepDefinition> Steps = new()
{
    // ... existing steps
    [TutorialStepId.NewStepName] = new(
        TutorialStepId.NewStepName,
        "Step Title",
        "Step description explaining what the user should do.",
        TutorialTargetElement.NewTargetElement,
        null, // secondary target (optional)
        TooltipPosition.Bottom,
        RequiresUserAction: false
    ),
};
```

### 4. Update Flow Logic

```csharp
// TutorialService.cs - DetermineNextStep()
TutorialStepId.PreviousStep => TutorialStepId.NewStepName,
TutorialStepId.NewStepName => TutorialStepId.NextStep,
```

### 5. Mark Element in XAML

```xml
<Button tutorial:TutorialTarget.Element="NewTargetElement" />
```

---

## Persistence

State stored via Settings key-value pairs:

- `Tutorial_Active` - Is tutorial currently running
- `Tutorial_CurrentStep` - Current step ID
- `Tutorial_Completed` - Has tutorial been completed
- `Tutorial_Skipped` - Was tutorial skipped

---

## Triggers and Context

### Application Triggers (AppTutorialTrigger)

| Trigger | Fired By | Steps That Listen |
|---------|----------|-------------------|
| DialogChanged | MainViewModel.OnCurrentDialogViewModelChanged() | PointToSourcesStep, PointToCalendarsStep, PointToSearchStep |
| SourceRefreshed | ManageSourcesViewModel.RefreshSourceData() | SourcesRefreshStep, WaitForRefreshStep |
| CalendarFormExpanded | ManageCalendarsViewModel (on AddCalendarFormExpandedEvent) | ClickAddCalendarStep |
| CalendarAdded | ManageCalendarsViewModel (on CalendarAddedEvent) | AddCalendarFormStep |
| CalendarSelected | ManageCalendarsViewModel.OnSelectedCalendarChanged() | ClickExistingCalendarStep |
| DateEntryAdded | ManageCalendarsViewModel (on DateEntryAddedEvent) | AddDaysStep |
| FilterSelected | SearchViewModel (on filter collection changes) | CalendarFilterStep, SourcesFilterStep |
| SearchPerformed | SearchViewModel.OnSearch() | SearchButtonStep |

### Context Properties (AvailabilityCompassContext)

| Property | Type | Used By | Updated When |
|----------|------|---------|--------------|
| HasSources | bool | - | DialogChanged |
| HasSourcesWithData | bool | WaitForRefreshStep | DialogChanged, SourceRefreshed |
| HasRefreshedSource | bool | SourcesRefreshStep, WaitForRefreshStep | SourceRefreshed |
| HasCalendars | bool | CalendarsOverview transition, step completion | DialogChanged, CalendarAdded |
| HasCalendarEntries | bool | CalendarViewOverview transition, AddDaysStep | CalendarSelected, DateEntryAdded |
| CurrentDialog | DialogType | PointToSourcesStep, PointToCalendarsStep, PointToSearchStep | DialogChanged |
| IsAddCalendarExpanded | bool | ClickAddCalendarStep | CalendarFormExpanded |
| IsCalendarSelected | bool | ClickExistingCalendarStep | CalendarSelected |
| HasSearchResults | bool | SearchButtonStep | SearchPerformed |
| HasSourceFilterSelected | bool | SourcesFilterStep, transition | FilterSelected |
| HasCalendarFilterSelected | bool | CalendarFilterStep | FilterSelected |


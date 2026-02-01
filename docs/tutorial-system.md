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

### Core Layer (`AvailabilityCompass.Core/Features/Tutorial/`)

| Component                  | File                        | Purpose                                                        |
| -------------------------- | --------------------------- | -------------------------------------------------------------- |
| **TutorialService**        | `TutorialService.cs`        | State machine - manages flow, persistence, event subscriptions |
| **TutorialStepRegistry**   | `TutorialStepRegistry.cs`   | All step definitions (titles, descriptions, targets)           |
| **TutorialViewModel**      | `TutorialViewModel.cs`      | UI binding layer with commands (Next, Back, Skip, Restart)     |
| **TutorialContext**        | `TutorialContext.cs`        | App state record for conditional branching                     |
| **ITutorialService**       | `ITutorialService.cs`       | Service interface                                              |
| **TutorialStepDefinition** | `TutorialStepDefinition.cs` | Step record + enums                                            |
| **TutorialStepId**         | `TutorialStepId.cs`         | Enum of all step identifiers                                   |
| **TutorialTargetElement**  | `TutorialTargetElement.cs`  | Enum of highlightable UI elements                              |
| **TutorialEvents**         | `Events/TutorialEvents.cs`  | Events (Started, StepChanged, Completed, Skipped)              |

### WPF Layer (`AvailabilityCompass.WpfClient/`)

| Component                    | File                                                    | Purpose                            |
| ---------------------------- | ------------------------------------------------------- | ---------------------------------- |
| **TutorialOverlay**          | `Shared/Controls/TutorialOverlay.xaml`                  | Visual tooltip + highlight control |
| **TutorialHighlightAdorner** | `Shared/Controls/TutorialHighlightAdorner.cs`           | Pulsing border effect on target    |
| **TutorialTarget**           | `Shared/Tutorial/TutorialTarget.cs`                     | Attached property to mark elements |
| **TutorialElementRegistry**  | `Shared/Tutorial/TutorialElementRegistry.cs`            | Tracks loaded UI elements          |
| **TutorialExtensions**       | `Application/DependencyInjection/TutorialExtensions.cs` | DI registration                    |

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

## Events

The tutorial subscribes to these app events for auto-advancement:

- `SourcesDataChangedEvent` - Source data refreshed
- `CalendarAddedEvent` - New calendar created
- `DateEntryAddedEvent` - Date entry added

The tutorial publishes these events:

- `TutorialStartedEvent`
- `TutorialStepChangedEvent`
- `TutorialCompletedEvent`
- `TutorialSkippedEvent`
- `TutorialContextChangedEvent`


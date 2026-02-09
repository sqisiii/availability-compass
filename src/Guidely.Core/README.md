# Guidely.Core

A powerful, framework-agnostic tutorial state machine library for .NET applications. Build interactive, step-by-step guided experiences with intelligent flow control.

## Features

- **Attribute-Driven Steps** - Define tutorial steps as simple classes with `[TutorialStep]`, `[TutorialTarget]`, and `[AutoAdvanceOn]` attributes
- **Conditional Transitions** - Create dynamic tutorial flows that adapt based on application state using a fluent builder API
- **Auto-Advance Triggers** - Steps automatically advance when users complete actions (button clicks, form submissions, etc.)
- **Context-Aware** - Track application state with strongly-typed context records for intelligent decision-making
- **Step Groups** - Organize steps into logical sections (Introduction, Setup, Features, etc.) with ordering support
- **Back Navigation** - Navigate to previous steps within the current group
- **Skippable Steps** - Steps that require user action can show alternate content and a Next button when their goal is already achieved, with configurable skip targets
- **Group-to-View Mapping** - Map tutorial groups to application views/pages for intelligent restart-from-current-view behavior
- **Persistence Ready** - Implement `ITutorialPersistence` to save/restore tutorial progress across sessions
- **Framework Agnostic** - Core logic works with any .NET UI framework (WPF, MAUI, Avalonia, Blazor)
- **Dependency Injection** - First-class support for Microsoft.Extensions.DependencyInjection
- **Auto-Start on First Run** - Optionally start the tutorial automatically when no persisted state exists

## Use Cases

- **Onboarding flows** - Guide new users through initial setup and configuration
- **Feature discovery** - Introduce users to new or advanced features
- **Interactive help** - Provide contextual guidance within complex workflows
- **Training modules** - Build step-by-step training for enterprise applications

## Installation

```xml
<PackageReference Include="Guidely.Core" Version="1.0.0" />
```

## Quick Start

### 1. Define Your Context

Create a record inheriting from `TutorialContextBase` to track application state:

```csharp
public record MyAppContext : TutorialContextBase
{
    public bool HasCompletedSetup { get; init; }
    public bool IsFeatureEnabled { get; init; }
}
```

### 2. Define Triggers and Groups

```csharp
public enum MyTrigger
{
    SetupCompleted,
    FeatureEnabled
}

public enum MyGroup
{
    Introduction,
    Setup,
    Features,
    Complete
}
```

### 3. Create Tutorial Steps

```csharp
[TutorialStep(StepIds.Welcome,
    Group = MyGroup.Introduction,
    Position = TooltipPosition.Center,
    Order = 0)]
public class WelcomeStep : ITutorialStepContent
{
    public string Title => "Welcome!";
    public string Description => "Let's get started with the tutorial.";
}
```

### 4. Configure Transitions

```csharp
public static class TutorialSetup
{
    public static void ConfigureTransitions(TransitionBuilder<MyAppContext> builder)
    {
        builder.From(StepIds.Welcome)
            .GoToIf(StepIds.Features, ctx => ctx.HasCompletedSetup)
            .GoTo(StepIds.Setup);

        builder.From(StepIds.Setup)
            .GoTo(StepIds.Features);

        builder.From(StepIds.Features)
            .GoTo(StepIds.Complete);
    }
}
```

### 5. Register Services

```csharp
services.AddGuidely<MyAppContext, MyTrigger, MyGroup>(builder =>
{
    builder.ScanStepsFromAssembly(typeof(WelcomeStep).Assembly);
    builder.ConfigureTransitions(TutorialSetup.ConfigureTransitions);
    builder.SetStartStep(StepIds.Welcome);
    builder.EnableAutoStart(); // Optional: auto-start tutorial on first run
});
```

## Core Concepts

### Tutorial Context

The context is an immutable record that captures application state. The tutorial service uses this state to evaluate conditional transitions and determine which step to show next.

```csharp
public record MyAppContext : TutorialContextBase
{
    public bool IsLoggedIn { get; init; }
    public int ItemCount { get; init; }
    public string CurrentPage { get; init; } = string.Empty;
}
```

Update context using immutable `with` expressions:

```csharp
tutorialService.UpdateContext(ctx => ctx with { IsLoggedIn = true });
```

### Tutorial Steps

Steps are classes decorated with `[TutorialStep]` that implement `ITutorialStepContent`:

```csharp
[TutorialStep("MyStep",
    Group = MyGroup.Features,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 1)]
[TutorialTarget("MyButton")]
[AutoAdvanceOn(MyTrigger.ButtonClicked)]
public class MyStep : ITutorialStepContent
{
    public string Title => "Click the Button";
    public string Description => "Click the highlighted button to continue.";
}
```

### Type-Safe Step IDs

Use static string constants for compile-time validation:

```csharp
public static class StepIds
{
    public const string Welcome = nameof(Welcome);
    public const string Setup = nameof(Setup);
    public const string Features = nameof(Features);
    public const string Complete = nameof(Complete);
}
```

### Transitions

Configure step flow using the fluent `TransitionBuilder`:

```csharp
// Unconditional transition
builder.From(StepIds.Welcome)
    .GoTo(StepIds.Setup);

// Conditional transition (evaluated first)
builder.From(StepIds.Welcome)
    .GoToIf(StepIds.Features, ctx => ctx.HasCompletedSetup)
    .GoTo(StepIds.Setup);  // Fallback if condition is false

// Skip transition (used when step is skippable)
builder.From(StepIds.Setup)
    .SkipTo(StepIds.Features)   // Where Next goes when CanSkip returns true
    .GoTo(StepIds.Features);    // Normal transition after auto-advance

// Explicit back target (overrides history-based back)
builder.From(StepIds.Summary)
    .BackTo(StepIds.Overview)
    .GoTo(StepIds.Complete);

// Disable back navigation for a step
builder.From(StepIds.Setup)
    .DisableBack()
    .GoTo(StepIds.Features);
```

### Auto-Advance Triggers

Steps can auto-advance when triggers are fired:

```csharp
// Simple trigger-based auto-advance
[AutoAdvanceOn(MyTrigger.ButtonClicked)]
public class ClickButtonStep : ITutorialStepContent { ... }

// Complex conditional auto-advance
[AutoAdvanceOn(MyTrigger.DataLoaded)]
public class WaitForDataStep : ITutorialStepContent, IConditionalAutoAdvance<MyAppContext, MyTrigger>
{
    public bool ShouldAutoAdvance(MyTrigger trigger, MyAppContext? previous, MyAppContext current)
        => trigger == MyTrigger.DataLoaded && current.ItemCount > 0;
}
```

Fire triggers from your application:

```csharp
tutorialService.FireTrigger(MyTrigger.ButtonClicked);

// Or with context update in same atomic operation
tutorialService.FireTrigger(MyTrigger.DataLoaded, ctx => ctx with { ItemCount = 5 });
```

### Skippable Steps

Steps that require user action can show alternate content and a Next button when their goal is already achieved. This is useful for tutorial reruns where some data already exists.

Implement `ITutorialStepSkippable<TContext>` on the step class:

```csharp
[TutorialStep(StepIds.RefreshData,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly)]
[TutorialTarget("RefreshButton")]
[AutoAdvanceOn(MyTrigger.DataRefreshed)]
public class RefreshDataStep : ITutorialStepContent, ITutorialStepSkippable<MyAppContext>
{
    public string Title => "Refresh Data";
    public string Description => "Click Refresh to load the latest data.";

    // When CanSkip returns true, the ViewModel shows SkipTitle/SkipDescription
    // and displays the Next button even though RequiresUserAction is true
    public bool CanSkip(MyAppContext context) => context.HasData;
    public string SkipTitle => "Refresh Data";
    public string SkipDescription => "You already have data loaded. Click Next to continue, or Refresh to update.";
}
```

Configure the skip target in transitions using `SkipTo()`:

```csharp
builder.From(StepIds.RefreshData)
    .SkipTo(StepIds.NextSection)    // Where Next goes when step is skippable
    .GoTo(StepIds.WaitForRefresh);  // Normal transition after auto-advance
```

When `CanSkip` returns true:

- The step shows `SkipTitle` and `SkipDescription` instead of the normal content
- The Next button appears even if `RequiresUserAction = true`
- Clicking Next navigates to the `SkipTo` target
- Auto-advance still works if the user performs the action
- Back navigation skips over steps whose `CanSkip` returns true and stops at group boundaries

### Back Navigation

Back navigation is group-bounded and skip-aware by default:

- The back button is hidden when at the first step of a group (group boundary)
- Steps whose `CanSkip` returns true are automatically skipped when going back

For custom back behavior, use `BackTo()` and `DisableBack()` in the transition builder:

```csharp
// Explicit back target — overrides history-based back
builder.From(StepIds.Summary)
    .BackTo(StepIds.Overview)
    .GoTo(StepIds.Complete);

// Disable back navigation entirely for a step
builder.From(StepIds.Confirmation)
    .DisableBack()
    .GoTo(StepIds.Done);
```

### Group-to-View Mapping

Map tutorial groups to application views/pages so that restart begins at the relevant group:

```csharp
builder.MapGroupToView(MyGroup.Settings, ctx => ctx.CurrentPage == "Settings");
builder.MapGroupToView(MyGroup.Dashboard, ctx => ctx.CurrentPage == "Dashboard");
```

When `RestartFromCurrentViewAsync()` is called, the service evaluates these conditions against the current context and restarts from the matching group. If no group matches, it falls back to a full restart.

### Groups

Groups organize steps into logical sections and determine step ordering:

```csharp
public enum MyGroup
{
    Introduction,  // Steps with Group = 0
    Setup,         // Steps with Group = 1
    Features,      // Steps with Group = 2
    Complete       // Steps with Group = 3
}
```

## API Reference

### ITutorialService&lt;TContext, TTrigger, TGroup&gt;

Main service interface for controlling the tutorial.

| Property                 | Type                    | Description                                                                              |
| ------------------------ | ----------------------- | ---------------------------------------------------------------------------------------- |
| `CurrentStepId`          | `string`                | ID of the current step                                                                   |
| `CurrentStep`            | `TutorialStepMetadata?` | Metadata for the current step                                                            |
| `CurrentGroup`           | `TGroup`                | Group of the current step                                                                |
| `IsTutorialActive`       | `bool`                  | Whether tutorial is running                                                              |
| `IsTutorialCompleted`    | `bool`                  | Whether tutorial is finished                                                             |
| `Context`                | `TContext`              | Current tutorial context                                                                 |
| `CurrentStepNumber`      | `int`                   | Current step number (1-based)                                                            |
| `TotalSteps`             | `int`                   | Total number of steps                                                                    |
| `CanGoBack`              | `bool`                  | Whether back navigation is available (within current group, considering skippable steps) |
| `IsCurrentStepSkippable` | `bool`                  | Whether the current step can be skipped                                                  |

| Method                                 | Description                                                    |
| -------------------------------------- | -------------------------------------------------------------- |
| `InitializeAsync()`                    | Load persisted state and initialize                            |
| `StartTutorial()`                      | Start or restart the tutorial                                  |
| `AdvanceStep()`                        | Move to the next step                                          |
| `GoBack()`                             | Go to the previous non-skippable step within the current group |
| `SkipStep()`                           | Navigate using the configured skip transition                  |
| `SkipTutorial()`                       | Skip the tutorial entirely                                     |
| `RestartTutorialAsync()`               | Restart from the beginning                                     |
| `RestartFromCurrentViewAsync()`        | Restart from the group matching the current view               |
| `FireTrigger(trigger, contextUpdate?)` | Fire a trigger with optional context update                    |
| `UpdateContext(update)`                | Update context without firing a trigger                        |
| `RestartGroup(group)`                  | Restart from the beginning of a group                          |

### Attributes

#### [TutorialStep]

| Property             | Type               | Default  | Description                                     |
| -------------------- | ------------------ | -------- | ----------------------------------------------- |
| `Id`                 | `string`           | Required | Unique step identifier                          |
| `Group`              | `object?`          | `null`   | Group enum value (e.g., `MyGroup.Introduction`) |
| `Position`           | `TooltipPosition`  | `Bottom` | Tooltip position relative to target             |
| `RequiresUserAction` | `bool`             | `false`  | Hides Next button when true                     |
| `ClickThroughMode`   | `ClickThroughMode` | `None`   | Overlay interaction mode                        |
| `Order`              | `int`              | `0`      | Order within group                              |

#### [TutorialTarget]

Specifies which UI element(s) to highlight. Can be applied multiple times for different targets.

```csharp
[TutorialTarget("SaveButton")]
[TutorialTarget("CancelButton")]
```

**Multiple Elements with Same Target Name**: When multiple UI elements share the same target name (e.g., buttons in an ItemsControl/ListView template), all matching elements will be highlighted simultaneously. This is useful for highlighting all instances of a repeated element.

```xml
<!-- All Refresh buttons in the list will be highlighted -->
<ItemsControl ItemsSource="{Binding Items}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Button Content="Refresh"
                    guidely:TutorialTarget.Element="RefreshButton" />
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

#### [AutoAdvanceOn]

Specifies triggers that can cause auto-advance. Can be applied multiple times.

```csharp
[AutoAdvanceOn(MyTrigger.ItemSaved)]
[AutoAdvanceOn(MyTrigger.ItemDeleted)]
```

### Enums

#### TooltipPosition

```csharp
public enum TooltipPosition
{
    Top,
    Bottom,
    Left,
    Right,
    Center
}
```

#### ClickThroughMode

```csharp
public enum ClickThroughMode
{
    None,       // Overlay blocks all interaction
    TargetOnly, // Only target element(s) can be clicked
    All         // All elements can be clicked through
}
```

### ITutorialPersistence

Implement this interface to persist tutorial state:

```csharp
public class MyPersistence : ITutorialPersistence
{
    public async Task<TutorialState?> LoadStateAsync(CancellationToken ct = default)
    {
        // Load from database, file, etc.
    }

    public async Task SaveStateAsync(TutorialState state, CancellationToken ct = default)
    {
        // Save to database, file, etc.
    }
}
```

Register before `AddGuidely`:

```csharp
services.AddGuidelyPersistence<MyPersistence>();
services.AddGuidely<...>(...);
```

## Dependency Injection

### AddGuidely&lt;TContext, TTrigger, TGroup&gt;

Registers the tutorial service, configuration, and view model.

```csharp
services.AddGuidely<MyAppContext, MyTrigger, MyGroup>(builder =>
{
    builder.ScanStepsFromAssembly(typeof(WelcomeStep).Assembly);
    builder.ConfigureTransitions(MySetup.ConfigureTransitions);
    builder.SetStartStep(StepIds.Welcome);
    builder.EnableAutoStart(); // Optional: auto-start on first run
    builder.UseStepFactory((sp, type) => sp.GetRequiredService(type)); // Optional
});
```

#### Builder Methods

| Method                             | Description                                                        |
| ---------------------------------- | ------------------------------------------------------------------ |
| `ScanStepsFromAssembly(assembly)`  | Scans an assembly for step classes marked with `[TutorialStep]`    |
| `ConfigureTransitions(action)`     | Configures transitions between steps using `TransitionBuilder`     |
| `SetStartStep(stepId)`             | Sets the starting step ID                                          |
| `EnableAutoStart()`                | Enables automatic tutorial start on first run (no persisted state) |
| `UseStepFactory(factory)`          | Sets a custom factory for creating step instances                  |
| `MapGroupToView(group, condition)` | Maps a group to a view condition for restart-from-current-view     |

### AddGuidelyPersistence&lt;TPersistence&gt;

Registers a custom persistence implementation:

```csharp
services.AddGuidelyPersistence<MyPersistence>();

// Or with factory
services.AddGuidelyPersistence(sp => new MyPersistence(sp.GetRequiredService<IDb>()));
```

## Complete Example

```csharp
// Context
public record OnboardingContext : TutorialContextBase
{
    public bool HasProfile { get; init; }
    public bool HasConnectedAccount { get; init; }
}

// Triggers
public enum OnboardingTrigger
{
    ProfileCreated,
    AccountConnected
}

// Groups
public enum OnboardingGroup
{
    Welcome,
    Profile,
    Account,
    Done
}

// Step IDs
public static class StepIds
{
    public const string Welcome = nameof(Welcome);
    public const string CreateProfile = nameof(CreateProfile);
    public const string ConnectAccount = nameof(ConnectAccount);
    public const string AllDone = nameof(AllDone);
}

// Steps
[TutorialStep(StepIds.Welcome, Group = OnboardingGroup.Welcome, Position = TooltipPosition.Center)]
public class WelcomeStep : ITutorialStepContent
{
    public string Title => "Welcome!";
    public string Description => "Let's set up your account.";
}

[TutorialStep(StepIds.CreateProfile,
    Group = OnboardingGroup.Profile,
    Position = TooltipPosition.Right,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly)]
[TutorialTarget("ProfileButton")]
[AutoAdvanceOn(OnboardingTrigger.ProfileCreated)]
public class CreateProfileStep : ITutorialStepContent, IConditionalAutoAdvance<OnboardingContext, OnboardingTrigger>
{
    public string Title => "Create Your Profile";
    public string Description => "Click to create your profile.";

    public bool ShouldAutoAdvance(OnboardingTrigger trigger, OnboardingContext? prev, OnboardingContext curr)
        => trigger == OnboardingTrigger.ProfileCreated && curr.HasProfile;
}

// Transitions
public static class OnboardingSetup
{
    public static void ConfigureTransitions(TransitionBuilder<OnboardingContext> builder)
    {
        builder.From(StepIds.Welcome)
            .GoToIf(StepIds.ConnectAccount, ctx => ctx.HasProfile)
            .GoTo(StepIds.CreateProfile);

        builder.From(StepIds.CreateProfile)
            .GoTo(StepIds.ConnectAccount);

        builder.From(StepIds.ConnectAccount)
            .GoTo(StepIds.AllDone);
    }
}

// Registration
services.AddGuidely<OnboardingContext, OnboardingTrigger, OnboardingGroup>(builder =>
{
    builder.ScanStepsFromAssembly(typeof(WelcomeStep).Assembly);
    builder.ConfigureTransitions(OnboardingSetup.ConfigureTransitions);
    builder.SetStartStep(StepIds.Welcome);
    builder.EnableAutoStart(); // Start tutorial automatically on first run
});
```

## License

MIT

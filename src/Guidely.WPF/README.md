# Guidely.WPF

WPF tutorial overlay control library for creating interactive, guided tutorials. Works with Guidely.Core to display tooltips, highlight UI elements, and manage user interaction during tutorials.

## Installation

```xml
<PackageReference Include="Guidely.Core" Version="1.0.0" />
<PackageReference Include="Guidely.WPF" Version="1.0.0" />
```

## Quick Start

### 1. Register Services

```csharp
// In your DI setup
services.AddGuidely<MyAppContext, MyTrigger, MyGroup>(builder => { ... });
services.AddGuidelyWpf(); // Optional - for future extensibility
```

### 2. Add the Tutorial Overlay

Add the `TutorialOverlay` control to your main window, binding it to the `TutorialViewModel`:

```xml
<Window xmlns:guidely="clr-namespace:Guidely.WPF.Controls;assembly=Guidely.WPF">
    <Grid>
        <!-- Your main content -->
        <ContentControl Content="{Binding CurrentPage}" />

        <!-- Tutorial overlay - must be on top of all content -->
        <guidely:TutorialOverlay DataContext="{Binding TutorialViewModel}" />
    </Grid>
</Window>
```

### 3. Mark Target Elements

Use the `TutorialTarget.Element` attached property to mark elements for highlighting:

```xml
<Window xmlns:guidely="clr-namespace:Guidely.WPF;assembly=Guidely.WPF">
    <Button Content="Save"
            guidely:TutorialTarget.Element="SaveButton" />
</Window>
```

### 4. Link Steps to Targets

In your step definitions, use `[TutorialTarget]` to specify which elements to highlight:

```csharp
[TutorialStep(StepIds.ClickSave, ...)]
[TutorialTarget("SaveButton")]
public class ClickSaveStep : ITutorialStepContent { ... }
```

## Core Concepts

### TutorialOverlay Control

The `TutorialOverlay` is a UserControl that displays:

- **Semi-transparent backdrop** - Dims the application to focus attention
- **Tooltip popup** - Shows step title, description, and navigation buttons
- **Highlights** - Draws attention to target elements with a glowing border

Place it at the root of your window/page, after all other content so it renders on top.

### Element Targeting

The attached property `TutorialTarget.Element` registers UI elements for highlighting:

```xml
<!-- Single target -->
<Button guidely:TutorialTarget.Element="MyButton" />

<!-- Multiple elements can share the same target name -->
<Button guidely:TutorialTarget.Element="ActionButtons" />
<Button guidely:TutorialTarget.Element="ActionButtons" />
```

When a step with `[TutorialTarget("MyButton")]` becomes active, the overlay automatically:

1. Finds the registered element
2. Adds a highlight border
3. Positions the tooltip relative to the element
4. Enables click-through if configured

### Multiple Targets

A step can highlight multiple elements by adding multiple `[TutorialTarget]` attributes:

```csharp
[TutorialStep(StepIds.SelectAndSave, Position = TooltipPosition.Right)]
[TutorialTarget("CalendarWidget")]   // First target - tooltip positions here
[TutorialTarget("SaveButton")]       // Also highlighted
public class SelectAndSaveStep : ITutorialStepContent { ... }
```

**Note:** When multiple targets are specified, the tooltip is positioned relative to the **first** `[TutorialTarget]` attribute. All targets will be highlighted with glowing borders.

### Click-Through Modes

The overlay supports three interaction modes controlled by `ClickThroughMode`:

| Mode         | Behavior                                        |
| ------------ | ----------------------------------------------- |
| `None`       | Overlay blocks all interaction (default)        |
| `TargetOnly` | Only target element(s) can be clicked           |
| `All`        | All elements can be clicked through the overlay |

Set the mode in your step definition:

```csharp
[TutorialStep(StepIds.ClickButton,
    ClickThroughMode = ClickThroughMode.TargetOnly)]
[TutorialTarget("MyButton")]
public class ClickButtonStep : ITutorialStepContent { ... }
```

### Tooltip Positioning

Tooltips are initially positioned relative to the target element using `TooltipPosition`:

```csharp
[TutorialStep(StepIds.MyStep, Position = TooltipPosition.Right)]
```

Available positions: `Top`, `Bottom`, `Left`, `Right`, `Center`

When set to `Center` (or when no target is specified), the tooltip centers in the overlay.

### Draggable Tooltips

Users can drag the tooltip to reposition it by clicking and dragging the header area. The `TooltipPosition` only sets the **initial position** when a step becomes active.

## API Reference

### TutorialOverlay Control

The main control that displays the tutorial UI.

**Properties:**

| Property        | Type     | Default | Description                                |
| --------------- | -------- | ------- | ------------------------------------------ |
| `TooltipWidth`  | `double` | `380`   | Approximate tooltip width for positioning  |
| `TooltipHeight` | `double` | `250`   | Approximate tooltip height for positioning |
| `TooltipMargin` | `double` | `16`    | Margin between tooltip and target          |

**DataContext Binding:**

Bind to `TutorialViewModel<TContext, TTrigger, TGroup>` from Guidely.Core. The overlay reads these properties via reflection:

- `IsVisible` - Whether to show the overlay
- `Title` - Current step title
- `Description` - Current step description
- `Targets` - List of target element names
- `TooltipPosition` - Initial tooltip position relative to target (user can drag to reposition)
- `ClickThroughMode` - Overlay interaction mode
- `ShowNextButton` - Whether to show the Next button
- `CanGoBack` - Whether Back button is available
- `StepProgress` - Progress text (e.g., "Step 3 of 10")
- `NextButtonText` - Text for Next button ("Next" or "Finish")
- Commands: `NextCommand`, `BackCommand`, `SkipCommand`

### TutorialTarget Attached Property

**Namespace:** `Guidely.WPF`

```xml
<Button guidely:TutorialTarget.Element="ElementName" />
```

When the element loads, it registers with `TutorialElementRegistry`. When it unloads, it automatically unregisters.

### TutorialElementRegistry

Static registry that tracks UI elements marked as tutorial targets.

**Methods:**

| Method                    | Description                                  |
| ------------------------- | -------------------------------------------- |
| `Register(name, element)` | Register an element (called automatically)   |
| `Unregister(name)`        | Unregister an element (called automatically) |
| `GetElement(name)`        | Get the element for a target name            |
| `GetAllTargetNames()`     | Get all registered target names              |
| `Clear()`                 | Clear all registrations                      |

**Events:**

| Event            | Description                                         |
| ---------------- | --------------------------------------------------- |
| `ElementChanged` | Fired when an element is registered or unregistered |

## Styling

The overlay uses resource brushes that can be overridden:

```xml
<Application.Resources>
    <!-- Override default colors -->
    <SolidColorBrush x:Key="GuidelyBackdropBrush" Color="#60000000" />
    <SolidColorBrush x:Key="GuidelyTooltipBackgroundBrush" Color="#FFFFFF" />
    <SolidColorBrush x:Key="GuidelyTooltipBorderBrush" Color="#3B82F6" />
    <SolidColorBrush x:Key="GuidelyPrimaryBrush" Color="#3B82F6" />
    <SolidColorBrush x:Key="GuidelyTextBrush" Color="#1F2937" />
    <SolidColorBrush x:Key="GuidelyMutedTextBrush" Color="#6B7280" />
</Application.Resources>
```

## XAML Namespace

Add the Guidely.WPF namespace to your XAML files:

```xml
<!-- For TutorialOverlay control -->
xmlns:guidely="clr-namespace:Guidely.WPF.Controls;assembly=Guidely.WPF"

<!-- For TutorialTarget attached property -->
xmlns:guidely="clr-namespace:Guidely.WPF;assembly=Guidely.WPF"
```

Or use a single namespace for both:

```xml
<Window xmlns:guidely="clr-namespace:Guidely.WPF;assembly=Guidely.WPF"
        xmlns:guidelyControls="clr-namespace:Guidely.WPF.Controls;assembly=Guidely.WPF">

    <Grid>
        <Button guidely:TutorialTarget.Element="MyButton" />
        <guidelyControls:TutorialOverlay DataContext="{Binding TutorialViewModel}" />
    </Grid>
</Window>
```

## Complete Example

**MainWindow.xaml:**

```xml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:guidely="clr-namespace:Guidely.WPF;assembly=Guidely.WPF"
        xmlns:guidelyControls="clr-namespace:Guidely.WPF.Controls;assembly=Guidely.WPF">

    <Grid>
        <!-- Main application content -->
        <StackPanel Margin="20">
            <TextBlock Text="My Application" FontSize="24" />

            <Button Content="Settings"
                    Margin="0,20,0,0"
                    guidely:TutorialTarget.Element="SettingsButton" />

            <Button Content="Save"
                    Margin="0,10,0,0"
                    guidely:TutorialTarget.Element="SaveButton" />

            <Button Content="Start Tutorial"
                    Margin="0,20,0,0"
                    Command="{Binding StartTutorialCommand}" />
        </StackPanel>

        <!-- Tutorial overlay - always on top -->
        <guidelyControls:TutorialOverlay DataContext="{Binding TutorialViewModel}" />
    </Grid>
</Window>
```

**MainViewModel.cs:**

```csharp
public class MainViewModel
{
    private readonly ITutorialService<MyAppContext, MyTrigger, MyGroup> _tutorialService;

    public TutorialViewModel<MyAppContext, MyTrigger, MyGroup> TutorialViewModel { get; }
    public ICommand StartTutorialCommand { get; }

    public MainViewModel(
        ITutorialService<MyAppContext, MyTrigger, MyGroup> tutorialService,
        TutorialViewModel<MyAppContext, MyTrigger, MyGroup> tutorialViewModel)
    {
        _tutorialService = tutorialService;
        TutorialViewModel = tutorialViewModel;
        StartTutorialCommand = new RelayCommand(() => _tutorialService.StartTutorial());
    }

    public async Task InitializeAsync()
    {
        await _tutorialService.InitializeAsync();
    }
}
```

**Tutorial Steps:**

```csharp
[TutorialStep(StepIds.Welcome,
    Group = MyGroup.Introduction,
    Position = TooltipPosition.Center)]
public class WelcomeStep : ITutorialStepContent
{
    public string Title => "Welcome!";
    public string Description => "This tutorial will show you around.";
}

[TutorialStep(StepIds.Settings,
    Group = MyGroup.Features,
    Position = TooltipPosition.Right,
    ClickThroughMode = ClickThroughMode.TargetOnly)]
[TutorialTarget("SettingsButton")]
public class SettingsStep : ITutorialStepContent
{
    public string Title => "Settings";
    public string Description => "Click here to access application settings.";
}

[TutorialStep(StepIds.Save,
    Group = MyGroup.Features,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly)]
[TutorialTarget("SaveButton")]
[AutoAdvanceOn(MyTrigger.DataSaved)]
public class SaveStep : ITutorialStepContent
{
    public string Title => "Save Your Work";
    public string Description => "Click Save to save your changes.";
}
```

## Requirements

- .NET 8.0+ or .NET Framework 4.7.2+
- WPF (Windows only)
- Guidely.Core package

## License

MIT

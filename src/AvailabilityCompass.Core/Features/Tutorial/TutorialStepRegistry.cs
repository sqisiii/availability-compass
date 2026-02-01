namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Registry containing all tutorial step definitions.
/// </summary>
public static class TutorialStepRegistry
{
    private static readonly Dictionary<TutorialStepId, TutorialStepDefinition> _stepsById;

    /// <summary>
    /// All tutorial steps in order.
    /// </summary>
    private static IReadOnlyList<TutorialStepDefinition> Steps { get; } = new List<TutorialStepDefinition>
    {
        // Phase 1: Sources
        new(
            TutorialStepId.Welcome,
            "Welcome to Availability Compass!",
            """
            This tutorial will guide you through setting up the application.

            You'll learn how to:
            • Refresh trip data from sources
            • Create calendars to define your availability
            • Search for trips that match your schedule

            Let's get started!
            """,
            TutorialTargetElement.None,
            null,
            TooltipPosition.Center
        ),

        new(
            TutorialStepId.SourcesDialogRefreshButtons,
            "Refresh Your Trip Data",
            """
            Each source card represents a different trip provider.

            To load the latest trip data, you can either:
            • Click 'Refresh All' button at the top to update all sources at once
            • Click the 'Refresh' button on individual source cards to update them one by one

            The progress bar shows the download status. Once refreshed, you'll see how many trip records were loaded and when they were last updated.
            """,
            TutorialTargetElement.RefreshAllButton,
            TutorialTargetElement.SourceCardRefreshButton,
            TooltipPosition.Bottom,
            RequiresUserAction: true
        ),

        new(
            TutorialStepId.PointToSourcesButton,
            "Open Sources Management",
            """
            Click the 'Sources' button in the header to open the source management dialog.

            Here you can:
            • Refresh trip data from various providers
            • Enable or disable specific sources
            • See statistics about loaded data
            """,
            TutorialTargetElement.SourcesHeaderButton,
            null,
            TooltipPosition.Bottom,
            RequiresUserAction: true
        ),

        // Phase 2: Transition
        new(
            TutorialStepId.PointToCalendarsButton,
            "Set Up Your Availability",
            """
            Excellent! Your trip data is now loaded.

            Next, let's set up your availability by creating calendars. Click the 'Calendars' button to define which dates you're available or unavailable for travel.
            """,
            TutorialTargetElement.CalendarsHeaderButton,
            null,
            TooltipPosition.Bottom,
            RequiresUserAction: true
        ),

        new(
            TutorialStepId.WaitForSourceRefresh,
            "Refresh Sources First",
            """
            Before setting up calendars, please refresh at least one source to load trip data.

            Click the 'Sources' button and use the Refresh buttons to download trip information.
            """,
            TutorialTargetElement.SourcesHeaderButton,
            null,
            TooltipPosition.Bottom,
            RequiresUserAction: true
        ),

        // Phase 3: Calendars Dialog
        new(
            TutorialStepId.CalendarsOverview,
            "Understanding Calendars",
            """
            Calendars help you define your availability for searching trips. There are two types:

            • Allowed Days Calendar: Only dates you add to this calendar will be considered when searching. Use this to mark specific vacation days or available periods.

            • Blocked Days Calendar: Dates you add will be excluded from search results. Use this to mark busy days, work commitments, or dates you cannot travel.

            You can create multiple calendars of each type and combine them when searching.
            """,
            TutorialTargetElement.None,
            null,
            TooltipPosition.Center
        ),

        new(
            TutorialStepId.ClickExistingCalendar,
            "Select a Calendar",
            """
            You already have calendars set up.

            Click on any calendar in this list to view and edit its dates. The calendar type (Allowed/Blocked) is shown below the name.

            The selected calendar's details will appear below.
            """,
            TutorialTargetElement.CalendarSelector,
            null,
            TooltipPosition.Bottom,
            RequiresUserAction: true
        ),

        new(
            TutorialStepId.ClickAddCalendarButton,
            "Create Your First Calendar",
            """
            Click the '+' button to create a new calendar.

            You'll be able to give it a name and choose whether it should mark:
            • Allowed days (dates you CAN travel)
            • Blocked days (dates you CANNOT travel)
            """,
            TutorialTargetElement.AddCalendarButton,
            null,
            TooltipPosition.Left,
            RequiresUserAction: true
        ),

        // Phase 4: Add Calendar Form
        new(
            TutorialStepId.AddCalendarFormExplanation,
            "Configure Your Calendar",
            """
            Fill in the calendar details:

            • Calendar Name: Enter a descriptive name (e.g., 'Summer Vacation', 'Work Trips', 'Blocked Weekends')

            • 'Only allow defined dates' checkbox:
              ✓ Checked = Allowed Days Calendar - Only the dates you add will be available for searching
              ☐ Unchecked = Blocked Days Calendar - The dates you add will be excluded from searches

            Click 'Create' when ready, or 'Cancel' to go back.
            """,
            TutorialTargetElement.AddCalendarForm,
            null,
            TooltipPosition.Right,
            RequiresUserAction: true
        ),

        // Phase 5: Calendar Days View
        new(
            TutorialStepId.CalendarViewOverview,
            "Managing Calendar Dates",
            """
            Now you can add dates to your calendar:

            • Edit button: Change the calendar name or type
            • Delete button: Remove this calendar entirely
            • Calendar widget: Click and drag to select dates, then use 'Add Dates' to save them

            The legend shows color coding:
            • Dark red = One-time dates
            • Salmon = Recurring dates

            Click on any colored date to edit that entry.
            """,
            TutorialTargetElement.CalendarWidget,
            null,
            TooltipPosition.Right
        ),

        new(
            TutorialStepId.AddDaysExplanation,
            "Add Date Entries",
            """
            After selecting dates on the calendar, click 'Add Dates' to create an entry.

            In the popup:
            • Description: Add a note explaining this date range (e.g., 'Family reunion', 'Conference')

            • 'Make this recurring' checkbox: Enable this for dates that repeat regularly. You can then set:
              - Frequency: How many days between occurrences (e.g., 7 for weekly, 14 for bi-weekly)
              - Repetitions: How many times it should repeat

            This is useful for regular commitments like weekly meetings or monthly events.
            """,
            TutorialTargetElement.AddDaysButton,
            null,
            TooltipPosition.Left,
            RequiresUserAction: true
        ),

        new(
            TutorialStepId.DateEntriesExplanation,
            "View and Edit Entries",
            """
            The Date Entries panel on the right shows all dates added to this calendar.

            Each entry displays:
            • The description you provided
            • The date or date range
            • Recurring pattern info (if applicable)

            Click on any entry to edit its details or delete it. You can also click directly on a colored date in the calendar widget to edit that specific entry.
            """,
            TutorialTargetElement.DateEntriesPanel,
            null,
            TooltipPosition.Left
        ),

        // Phase 6: Search View
        new(
            TutorialStepId.PointToSearchView,
            "Return to Search",
            """
            Great job setting up your calendars!

            Now let's use them to find matching trips. Close this dialog to return to the main search view.
            """,
            TutorialTargetElement.None,
            null,
            TooltipPosition.Center,
            RequiresUserAction: true
        ),

        new(
            TutorialStepId.CalendarFilterExplanation,
            "Filter by Calendar",
            """
            Click 'Calendars' to expand this section and select which calendars to apply when searching.

            You can select multiple calendars:
            • Green checkmark calendars = Allowed days (results must fall within these dates)
            • Orange checkmark calendars = Blocked days (results will exclude these dates)

            If no calendars are selected, all dates are considered available.
            """,
            TutorialTargetElement.CalendarFilterSection,
            null,
            TooltipPosition.Bottom,
            RequiresUserAction: true
        ),

        new(
            TutorialStepId.SourcesFilterExplanation,
            "Filter by Source",
            """
            Click 'Sources' to expand this section.

            Toggle source cards to include or exclude specific trip providers from your search results.

            Disabled sources (those you turned off in source management) appear grayed out.
            """,
            TutorialTargetElement.SourcesFilterSection,
            null,
            TooltipPosition.Bottom,
            RequiresUserAction: true
        ),

        new(
            TutorialStepId.SourceFilterOptionsExplanation,
            "Source-Specific Filters",
            """
            When you select a source, additional filter options may appear below.

            Each source has different criteria you can filter by, such as:
            • Destination preferences
            • Trip type (cruise, tour, etc.)
            • Price ranges
            • Other source-specific options

            These filters are optional and help narrow down results.
            """,
            TutorialTargetElement.SourceFilterOptions,
            null,
            TooltipPosition.Bottom
        ),

        new(
            TutorialStepId.FiltersExplanation,
            "Global Filters",
            """
            Click 'Filters' to access global search options:

            • Search phrase: Find trips containing specific text in their title or description
            • Start Date / End Date: Limit results to trips within a specific date range

            These filters apply across all selected sources.
            """,
            TutorialTargetElement.FiltersSection,
            null,
            TooltipPosition.Bottom
        ),

        new(
            TutorialStepId.SearchButtonExplanation,
            "Run Your Search",
            """
            Click the 'Search' button to find trips matching your criteria.

            The search will query all selected sources and filter results based on your calendar and filter settings.
            """,
            TutorialTargetElement.SearchButton,
            null,
            TooltipPosition.Bottom,
            RequiresUserAction: true
        ),

        new(
            TutorialStepId.ResultsExplanation,
            "Search Results",
            """
            After clicking 'Search', matching trips appear as cards below.

            Each card shows:
            • Source name and language
            • Destination and travel dates
            • Trip title and details
            • Additional information specific to each source

            Click on any result card to open the trip's page on the source website in your browser. You can also sort results using the dropdown at the top right.
            """,
            TutorialTargetElement.ResultsSection,
            null,
            TooltipPosition.Top
        ),

        // Completion
        new(
            TutorialStepId.TutorialComplete,
            "Tutorial Complete!",
            """
            Congratulations! You've learned how to:

            ✓ Refresh trip data from sources
            ✓ Create and manage availability calendars
            ✓ Add one-time and recurring date entries
            ✓ Search for trips matching your schedule
            ✓ Filter results by source and criteria

            You can restart this tutorial anytime from the header menu.

            Happy trip hunting!
            """,
            TutorialTargetElement.None,
            null,
            TooltipPosition.Center
        )
    };

    static TutorialStepRegistry()
    {
        _stepsById = Steps.ToDictionary(s => s.Id);
    }

    /// <summary>
    /// Total number of steps (excluding TutorialComplete for progress display).
    /// </summary>
    public static int TotalStepsForProgress => Steps.Count - 1; // Exclude TutorialComplete

    /// <summary>
    /// Gets a step definition by its ID.
    /// </summary>
    public static TutorialStepDefinition GetStep(TutorialStepId id) =>
        _stepsById.TryGetValue(id, out var step)
            ? step
            : throw new ArgumentException($"Unknown tutorial step: {id}", nameof(id));

    /// <summary>
    /// Gets the index of a step (0-based).
    /// </summary>
    public static int GetStepIndex(TutorialStepId id)
    {
        for (var i = 0; i < Steps.Count; i++)
        {
            if (Steps[i].Id == id)
                return i;
        }

        return -1;
    }
}
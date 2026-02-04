using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.Welcome, Group = AppTutorialGroup.Introduction, Position = TooltipPosition.Center, Order = 0)]
public class WelcomeStep : ITutorialStepContent
{
    public string Title => "Welcome to Availability Compass!";

    public string Description => """
                                 This tutorial will guide you through setting up the application.

                                 You'll learn how to:
                                 • Refresh trip data from sources
                                 • Create calendars to define your availability
                                 • Search for trips that match your schedule

                                 Let's get started!
                                 """;
}
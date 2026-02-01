namespace AvailabilityCompass.Core.Features.Tutorial.Events;

/// <summary>
/// Published when the tutorial is started or restarted.
/// </summary>
public record TutorialStartedEvent;

/// <summary>
/// Published when the tutorial step changes.
/// </summary>
public record TutorialStepChangedEvent;

/// <summary>
/// Published when the tutorial is completed.
/// </summary>
public record TutorialCompletedEvent;

/// <summary>
/// Published when the user skips the tutorial.
/// </summary>
public record TutorialSkippedEvent;

/// <summary>
/// Published when the tutorial context (app state) changes.
/// </summary>
public record TutorialContextChangedEvent;

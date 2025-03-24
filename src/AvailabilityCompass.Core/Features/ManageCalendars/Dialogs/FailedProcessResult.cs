namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

/// <summary>
/// Represents a failed dialog processing result. Implements the <see cref="IProcessResult"/> interface.
/// </summary>
public class FailedProcessResult : IProcessResult
{
    public bool IsSuccess => false;
}
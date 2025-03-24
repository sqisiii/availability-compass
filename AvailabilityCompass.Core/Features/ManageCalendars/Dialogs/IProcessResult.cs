namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

/// <summary>
/// Represents the result of a dialog process operation.
/// </summary>
public interface IProcessResult
{
    bool IsSuccess { get; }
}
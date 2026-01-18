using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Manages the date entry editor panel state and CRUD operations.
/// </summary>
public interface IDateEntryEditorController : INotifyPropertyChanged
{
    // Editor State
    bool IsEditorOpen { get; }
    bool IsEditMode { get; }
    string EditorTitle { get; }

    // Form Fields
    string EditorDescription { get; set; }
    string? EditorStartDateString { get; set; }
    int EditorDuration { get; set; }
    bool EditorIsRecurring { get; set; }
    int? EditorFrequency { get; set; }
    int EditorRepetitions { get; set; }

    // Multi-select State
    ObservableCollection<DetectedSelection> EditorDetectedSelections { get; }

    /// <summary>
    /// Opens the editor for a collection of selected dates from calendar multi-select.
    /// </summary>
    /// <param name="selectedDates">The dates selected by the user.</param>
    void OpenForSelectedDates(IList? selectedDates);

    /// <summary>
    /// Opens the editor when a single date is clicked on the calendar.
    /// </summary>
    /// <param name="date">The clicked date.</param>
    /// <param name="existingEntries">Existing date entries to check for conflicts.</param>
    void OpenForDateClick(DateOnly date, IEnumerable<DateEntryViewModel> existingEntries);

    /// <summary>
    /// Opens the editor in edit mode for an existing date entry.
    /// </summary>
    /// <param name="entry">The date entry to edit.</param>
    void OpenForEdit(DateEntryViewModel entry);

    /// <summary>
    /// Saves the current editor state as a new or updated date entry.
    /// </summary>
    /// <param name="calendarId">The ID of the calendar to save the entry to.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task SaveAsync(Guid calendarId);

    /// <summary>
    /// Deletes the currently edited date entry.
    /// </summary>
    /// <param name="calendarId">The ID of the calendar containing the entry.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteAsync(Guid calendarId);

    /// <summary>
    /// Closes the editor and resets its state.
    /// </summary>
    void Close();
}

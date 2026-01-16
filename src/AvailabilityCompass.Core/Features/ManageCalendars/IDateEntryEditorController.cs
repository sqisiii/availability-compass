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

    // Operations
    void OpenForSelectedDates(IList? selectedDates);
    void OpenForDateClick(DateOnly date, IEnumerable<DateEntryViewModel> existingEntries);
    void OpenForEdit(DateEntryViewModel entry);
    Task SaveAsync(Guid calendarId);
    Task DeleteAsync(Guid calendarId);
    void Close();
}

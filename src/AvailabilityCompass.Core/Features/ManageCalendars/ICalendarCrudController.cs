using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Manages inline calendar CRUD operations and their associated UI state.
/// </summary>
public interface ICalendarCrudController : INotifyPropertyChanged
{
    // Add Calendar State
    bool IsAddCalendarExpanded { get; }
    string NewCalendarName { get; set; }
    bool NewCalendarIsOnly { get; set; }

    // Edit Calendar State
    bool IsEditCalendarExpanded { get; }
    string EditCalendarName { get; set; }
    bool EditCalendarIsOnly { get; set; }

    // Delete Confirmation State
    bool IsDeleteConfirmationOpen { get; }
    string DeleteCalendarName { get; }

    // Add Calendar Operations
    Task AddCalendarAsync();
    void ExpandAddCalendar(Action? onBeforeExpand = null);
    void CancelAddCalendar();

    // Edit Calendar Operations
    void StartCalendarEdit(CalendarViewModel calendar);
    Task SaveCalendarEditAsync();
    void CancelCalendarEdit();

    // Delete Calendar Operations
    void StartDeleteCalendar(CalendarViewModel calendar);
    Task ConfirmDeleteCalendarAsync();
    void CancelDeleteCalendar();
}

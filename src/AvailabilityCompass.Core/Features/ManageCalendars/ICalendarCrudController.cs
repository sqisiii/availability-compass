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

    /// <summary>
    /// Adds a new calendar using the current add form state.
    /// </summary>
    /// <returns>A task representing the asynchronous add operation.</returns>
    Task AddCalendarAsync();

    /// <summary>
    /// Expands the add calendar form panel.
    /// </summary>
    /// <param name="onBeforeExpand">Optional action to execute before expanding.</param>
    void ExpandAddCalendar(Action? onBeforeExpand = null);

    /// <summary>
    /// Cancels the add calendar operation and collapses the form.
    /// </summary>
    void CancelAddCalendar();

    /// <summary>
    /// Starts editing an existing calendar.
    /// </summary>
    /// <param name="calendar">The calendar to edit.</param>
    void StartCalendarEdit(CalendarViewModel calendar);

    /// <summary>
    /// Saves the current calendar edit.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task SaveCalendarEditAsync();

    /// <summary>
    /// Cancels the current calendar edit operation.
    /// </summary>
    void CancelCalendarEdit();

    /// <summary>
    /// Starts the delete confirmation flow for a calendar.
    /// </summary>
    /// <param name="calendar">The calendar to delete.</param>
    void StartDeleteCalendar(CalendarViewModel calendar);

    /// <summary>
    /// Confirms and executes the calendar deletion.
    /// </summary>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task ConfirmDeleteCalendarAsync();

    /// <summary>
    /// Cancels the delete confirmation dialog.
    /// </summary>
    void CancelDeleteCalendar();
}

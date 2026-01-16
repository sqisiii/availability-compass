using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;
using CommunityToolkit.Mvvm.ComponentModel;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Manages inline calendar CRUD operations and their associated UI state.
/// </summary>
public partial class CalendarCrudController : ObservableObject, ICalendarCrudController
{
    private readonly IMediator _mediator;

    [ObservableProperty]
    private string _deleteCalendarName = string.Empty;

    [ObservableProperty]
    private bool _editCalendarIsOnly;

    [ObservableProperty]
    private string _editCalendarName = string.Empty;

    private Guid? _editingCalendarId;

    [ObservableProperty]
    private bool _isAddCalendarExpanded;

    [ObservableProperty]
    private bool _isDeleteConfirmationOpen;

    [ObservableProperty]
    private bool _isEditCalendarExpanded;

    [ObservableProperty]
    private bool _newCalendarIsOnly;

    [ObservableProperty]
    private string _newCalendarName = string.Empty;

    private Guid? _pendingDeleteCalendarId;

    public CalendarCrudController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task AddCalendarAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCalendarName))
        {
            return;
        }

        await _mediator.Send(new AddCalendarToDbRequest(NewCalendarName, NewCalendarIsOnly));

        IsAddCalendarExpanded = false;
        NewCalendarName = string.Empty;
        NewCalendarIsOnly = false;
    }

    public void ExpandAddCalendar(Action? onBeforeExpand = null)
    {
        onBeforeExpand?.Invoke();
        IsAddCalendarExpanded = true;
    }

    public void CancelAddCalendar()
    {
        IsAddCalendarExpanded = false;
        NewCalendarName = string.Empty;
        NewCalendarIsOnly = false;
    }

    public void StartCalendarEdit(CalendarViewModel calendar)
    {
        _editingCalendarId = calendar.CalendarId;
        EditCalendarName = calendar.Name;
        EditCalendarIsOnly = calendar.IsOnly;
        IsEditCalendarExpanded = true;
    }

    public async Task SaveCalendarEditAsync()
    {
        if (_editingCalendarId is null || string.IsNullOrWhiteSpace(EditCalendarName))
        {
            return;
        }

        await _mediator.Send(new UpdateCalendarInDbRequest(
            _editingCalendarId.Value,
            EditCalendarName,
            EditCalendarIsOnly));

        ResetEditState();
    }

    public void CancelCalendarEdit()
    {
        ResetEditState();
    }

    public void StartDeleteCalendar(CalendarViewModel calendar)
    {
        _pendingDeleteCalendarId = calendar.CalendarId;
        DeleteCalendarName = calendar.Name;
        IsDeleteConfirmationOpen = true;
    }

    public async Task ConfirmDeleteCalendarAsync()
    {
        if (_pendingDeleteCalendarId is null)
        {
            return;
        }

        await _mediator.Send(new DeleteCalendarFromDbRequest(_pendingDeleteCalendarId.Value));
        ResetDeleteState();
    }

    public void CancelDeleteCalendar()
    {
        ResetDeleteState();
    }

    private void ResetEditState()
    {
        IsEditCalendarExpanded = false;
        _editingCalendarId = null;
        EditCalendarName = string.Empty;
        EditCalendarIsOnly = false;
    }

    private void ResetDeleteState()
    {
        IsDeleteConfirmationOpen = false;
        _pendingDeleteCalendarId = null;
        DeleteCalendarName = string.Empty;
    }
}
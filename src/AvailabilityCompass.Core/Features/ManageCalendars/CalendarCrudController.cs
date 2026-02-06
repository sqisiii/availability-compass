using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Events;
using AvailabilityCompass.Core.Shared.EventBus;
using CommunityToolkit.Mvvm.ComponentModel;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Manages inline calendar CRUD operations and their associated UI state.
/// </summary>
public partial class CalendarCrudController : ObservableObject, ICalendarCrudController
{
    private readonly IEventBus _eventBus;
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

    public CalendarCrudController(IMediator mediator, IEventBus eventBus)
    {
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public bool WasCalendarDeleted { get; private set; }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void ExpandAddCalendar(Action? onBeforeExpand = null)
    {
        onBeforeExpand?.Invoke();
        IsAddCalendarExpanded = true;
        _eventBus.Publish(new AddCalendarFormExpandedEvent());
    }

    /// <inheritdoc />
    public void CancelAddCalendar()
    {
        IsAddCalendarExpanded = false;
        NewCalendarName = string.Empty;
        NewCalendarIsOnly = false;
    }

    /// <inheritdoc />
    public void StartCalendarEdit(CalendarViewModel calendar)
    {
        _editingCalendarId = calendar.CalendarId;
        EditCalendarName = calendar.Name;
        EditCalendarIsOnly = calendar.IsOnly;
        IsEditCalendarExpanded = true;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void CancelCalendarEdit()
    {
        ResetEditState();
    }

    /// <inheritdoc />
    public void StartDeleteCalendar(CalendarViewModel calendar)
    {
        _pendingDeleteCalendarId = calendar.CalendarId;
        DeleteCalendarName = calendar.Name;
        IsDeleteConfirmationOpen = true;
    }

    /// <inheritdoc />
    public async Task ConfirmDeleteCalendarAsync()
    {
        if (_pendingDeleteCalendarId is null)
        {
            return;
        }

        await _mediator.Send(new DeleteCalendarFromDbRequest(_pendingDeleteCalendarId.Value));
        WasCalendarDeleted = true;
        ResetDeleteState();
    }

    /// <inheritdoc />
    public void CancelDeleteCalendar()
    {
        WasCalendarDeleted = false;
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
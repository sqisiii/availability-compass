using System.Collections;
using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddDateEntryRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteDateEntryRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateDateEntryRequest;
using CommunityToolkit.Mvvm.ComponentModel;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Manages the date entry editor panel state and CRUD operations.
/// </summary>
public partial class DateEntryEditorController : ObservableObject, IDateEntryEditorController
{
    private const string TitleAddDateEntry = "Add Date Entry";
    private const string TitleAddPeriod = "Add Period";
    private const string TitleEditDateEntry = "Edit Date Entry";
    private const string TitleEditPeriod = "Edit Period";
    private const string DateFormat = "yyyy-MM-dd";

    private readonly IDateSelectionParser _dateSelectionParser;
    private readonly IMediator _mediator;

    private Guid? _editingEntryId;

    [ObservableProperty]
    private string _editorDescription = string.Empty;

    [ObservableProperty]
    private ObservableCollection<DetectedSelection> _editorDetectedSelections = [];

    [ObservableProperty]
    private int _editorDuration = 1;

    [ObservableProperty]
    private int? _editorFrequency;

    [ObservableProperty]
    private bool _editorIsRecurring;

    [ObservableProperty]
    private int _editorRepetitions;

    [ObservableProperty]
    private string? _editorStartDateString;

    [ObservableProperty]
    private string _editorTitle = TitleAddDateEntry;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private bool _isEditorOpen;

    private List<DetectedSelection>? _pendingSelections;

    public DateEntryEditorController(IMediator mediator, IDateSelectionParser dateSelectionParser)
    {
        _mediator = mediator;
        _dateSelectionParser = dateSelectionParser;
    }

    /// <inheritdoc />
    public void OpenForSelectedDates(IList? selectedDates)
    {
        if (selectedDates is null || selectedDates.Count == 0)
        {
            return;
        }

        var dates = selectedDates.Cast<DateTime>().ToList();
        _pendingSelections = _dateSelectionParser.ParseSelectedDates(dates);

        if (_pendingSelections.Count == 0)
        {
            return;
        }

        EditorDetectedSelections.Clear();
        foreach (var selection in _pendingSelections)
        {
            EditorDetectedSelections.Add(selection);
        }

        EditorTitle = _pendingSelections.Count > 1
            ? $"Add {_pendingSelections.Count} Date Entries"
            : _pendingSelections[0].IsPeriod
                ? TitleAddPeriod
                : TitleAddDateEntry;

        EditorStartDateString = _pendingSelections[0].StartDate.ToString(DateFormat);
        EditorDescription = string.Empty;
        EditorIsRecurring = false;
        EditorDuration = _pendingSelections[0].Duration;
        EditorFrequency = null;
        EditorRepetitions = 0;
        IsEditMode = false;
        _editingEntryId = null;

        IsEditorOpen = true;
    }

    /// <inheritdoc />
    public void OpenForDateClick(DateOnly date, IEnumerable<DateEntryViewModel> existingEntries)
    {
        var existingEntry = FindEntryByDate(date, existingEntries);

        if (existingEntry is not null)
        {
            OpenForEdit(existingEntry);
        }
        else
        {
            PrepareNewEntry(date);
            IsEditorOpen = true;
        }
    }

    /// <inheritdoc />
    public void OpenForEdit(DateEntryViewModel entry)
    {
        IsEditMode = true;
        _editingEntryId = entry.DateEntryId;
        _pendingSelections = null;

        EditorDetectedSelections.Clear();
        if (entry.StartDate is { } startDate)
        {
            EditorDetectedSelections.Add(new DetectedSelection(startDate, entry.Duration));
        }

        EditorTitle = entry.Duration > 1 ? TitleEditPeriod : TitleEditDateEntry;
        EditorDescription = entry.Description;
        EditorStartDateString = entry.StartDateString;
        EditorIsRecurring = entry.IsRecurring;
        EditorDuration = entry.Duration;
        EditorFrequency = entry.Frequency;
        EditorRepetitions = entry.NumberOfRepetitions;

        IsEditorOpen = true;
    }

    /// <inheritdoc />
    public async Task SaveAsync(Guid calendarId)
    {
        if (_pendingSelections is { Count: > 0 })
        {
            foreach (var selection in _pendingSelections)
            {
                await _mediator.Send(new AddDateEntryToDbRequest(
                    calendarId,
                    EditorDescription,
                    selection.StartDate,
                    EditorIsRecurring,
                    selection.Duration,
                    EditorIsRecurring ? EditorFrequency : null,
                    EditorIsRecurring ? EditorRepetitions : 0));
            }

            _pendingSelections = null;
            Close();
            return;
        }

        if (string.IsNullOrWhiteSpace(EditorStartDateString))
        {
            return;
        }

        if (!DateOnly.TryParse(EditorStartDateString, out var startDate))
        {
            return;
        }

        if (IsEditMode && _editingEntryId.HasValue)
        {
            await _mediator.Send(new UpdateDateEntryInDbRequest(
                calendarId,
                _editingEntryId.Value,
                EditorDescription,
                startDate,
                EditorIsRecurring,
                EditorIsRecurring ? EditorDuration : 1,
                EditorIsRecurring ? EditorFrequency : null,
                EditorIsRecurring ? EditorRepetitions : 0));
        }
        else
        {
            await _mediator.Send(new AddDateEntryToDbRequest(
                calendarId,
                EditorDescription,
                startDate,
                EditorIsRecurring,
                EditorIsRecurring ? EditorDuration : 1,
                EditorIsRecurring ? EditorFrequency : null,
                EditorIsRecurring ? EditorRepetitions : 0));
        }

        Close();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid calendarId)
    {
        if (!IsEditMode || !_editingEntryId.HasValue)
        {
            return;
        }

        await _mediator.Send(new DeleteDateEntryFromDbRequest(calendarId, _editingEntryId.Value));
        Close();
    }

    /// <inheritdoc />
    public void Close()
    {
        IsEditorOpen = false;
        _editingEntryId = null;
        _pendingSelections = null;
        EditorDetectedSelections.Clear();
        EditorDescription = string.Empty;
        EditorStartDateString = null;
        EditorIsRecurring = false;
        EditorDuration = 1;
        EditorFrequency = null;
        EditorRepetitions = 0;
        IsEditMode = false;
        EditorTitle = TitleAddDateEntry;
    }

    private void PrepareNewEntry(DateOnly date)
    {
        IsEditMode = false;
        _editingEntryId = null;
        _pendingSelections = null;

        EditorDetectedSelections.Clear();
        EditorDetectedSelections.Add(new DetectedSelection(date, 1));

        EditorTitle = TitleAddDateEntry;
        EditorDescription = string.Empty;
        EditorStartDateString = date.ToString(DateFormat);
        EditorIsRecurring = false;
        EditorDuration = 1;
        EditorFrequency = null;
        EditorRepetitions = 0;
    }

    private static DateEntryViewModel? FindEntryByDate(DateOnly date, IEnumerable<DateEntryViewModel> entries)
    {
        return entries.FirstOrDefault(e => IsDateWithinEntry(date, e));
    }

    public static bool IsDateWithinEntry(DateOnly date, DateEntryViewModel entry)
    {
        if (entry.IsRecurring && entry.Frequency.HasValue)
        {
            var currentStart = entry.StartDate;
            for (var i = 0; i <= entry.NumberOfRepetitions; i++)
            {
                var periodEnd = currentStart.AddDays(entry.Duration - 1);
                if (date >= currentStart && date <= periodEnd)
                {
                    return true;
                }

                currentStart = currentStart.AddDays(entry.Frequency.Value);
            }

            return false;
        }

        var endDate = entry.StartDate.AddDays(entry.Duration - 1);
        return date >= entry.StartDate && date <= endDate;
    }
}
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddDateEntryRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteDateEntryRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateDateEntryRequest;
using CommunityToolkit.Mvvm.ComponentModel;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Manages the date entry editor panel state and CRUD operations.
/// </summary>
public partial class DateEntryEditorController : ObservableValidator, IDateEntryEditorController, IDisposable
{
    private const string TitleAddDateEntry = "Add Date Entry";
    private const string TitleAddPeriod = "Add Period";
    private const string TitleEditDateEntry = "Edit Date Entry";
    private const string TitleEditPeriod = "Edit Period";
    private const string DateFormat = "yyyy-MM-dd";

    private readonly IDateSelectionParser _dateSelectionParser;
    private readonly EventHandler<DataErrorsChangedEventArgs>? _errorsChangedHandler;
    private readonly IMediator _mediator;

    private Guid? _editingEntryId;

    [ObservableProperty]
    private string _editorDescription = string.Empty;

    [ObservableProperty]
    private ObservableCollection<DetectedSelection> _editorDetectedSelections = [];

    [ObservableProperty]
    private int _editorDuration = 1;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [FrequencyValidation]
    private int? _editorFrequency;

    [ObservableProperty]
    private bool _editorIsRecurring;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RepetitionsValidation]
    private int? _editorRepetitions;

    [ObservableProperty]
    private string? _editorStartDateString;

    [ObservableProperty]
    private string _editorTitle = TitleAddDateEntry;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private bool _isEditorOpen;

    private List<DetectedSelection>? _pendingSelections;

    public string? FrequencyError => GetErrors(nameof(EditorFrequency)).Cast<object>().FirstOrDefault()?.ToString();
    public string? RepetitionsError => GetErrors(nameof(EditorRepetitions)).Cast<object>().FirstOrDefault()?.ToString();

    public DateEntryEditorController(IMediator mediator, IDateSelectionParser dateSelectionParser)
    {
        _mediator = mediator;
        _dateSelectionParser = dateSelectionParser;
        _errorsChangedHandler = OnErrorsChanged;
        ErrorsChanged += _errorsChangedHandler;
    }

    public void Dispose()
    {
        ErrorsChanged -= _errorsChangedHandler;
    }

    private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasErrors));
        OnPropertyChanged(nameof(FrequencyError));
        OnPropertyChanged(nameof(RepetitionsError));
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
        // Treat recurring with 0 repetitions as a single date
        var effectiveIsRecurring = EditorIsRecurring && (EditorRepetitions ?? 0) > 0;

        if (_pendingSelections is { Count: > 0 })
        {
            foreach (var selection in _pendingSelections)
            {
                await _mediator.Send(new AddDateEntryToDbRequest(
                    calendarId,
                    EditorDescription,
                    selection.StartDate,
                    effectiveIsRecurring,
                    selection.Duration,
                    effectiveIsRecurring ? EditorFrequency : null,
                    effectiveIsRecurring ? EditorRepetitions ?? 0 : 0));
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
                effectiveIsRecurring,
                effectiveIsRecurring ? EditorDuration : 1,
                effectiveIsRecurring ? EditorFrequency : null,
                effectiveIsRecurring ? EditorRepetitions ?? 0 : 0));
        }
        else
        {
            await _mediator.Send(new AddDateEntryToDbRequest(
                calendarId,
                EditorDescription,
                startDate,
                effectiveIsRecurring,
                effectiveIsRecurring ? EditorDuration : 1,
                effectiveIsRecurring ? EditorFrequency : null,
                effectiveIsRecurring ? EditorRepetitions ?? 0 : 0));
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
        WasEntryDeleted = true;
        Close();
    }

    /// <inheritdoc />
    public bool WasEntryDeleted { get; private set; }

    /// <inheritdoc />
    public void Close()
    {
        IsEditorOpen = false;
        WasEntryDeleted = false;
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

    /// <summary>
    /// Sets the default frequency when recurring mode is enabled.
    /// </summary>
    partial void OnEditorIsRecurringChanged(bool value)
    {
        if (value && (EditorFrequency is null || EditorFrequency < EditorDuration + 1))
        {
            EditorFrequency = EditorDuration + 1;
        }

        ValidateProperty(EditorFrequency, nameof(EditorFrequency));
        ValidateProperty(EditorRepetitions, nameof(EditorRepetitions));
    }

    /// <summary>
    /// Updates a frequency minimum when duration changes and re-validates.
    /// </summary>
    partial void OnEditorDurationChanged(int value)
    {
        if (EditorIsRecurring && (EditorFrequency is null || EditorFrequency < value + 1))
        {
            EditorFrequency = value + 1;
        }

        ValidateProperty(EditorFrequency, nameof(EditorFrequency));
    }
}
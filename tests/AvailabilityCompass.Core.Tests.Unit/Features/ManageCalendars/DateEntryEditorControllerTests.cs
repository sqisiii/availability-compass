using System.Collections;
using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddDateEntryRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteDateEntryRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateDateEntryRequest;
using MediatR;
using NSubstitute;
using Shouldly;

namespace AvailabilityCompass.Core.Tests.Features.ManageCalendars;

public class DateEntryEditorControllerTests
{
    private readonly IDateSelectionParser _dateSelectionParser = Substitute.For<IDateSelectionParser>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly DateEntryEditorController _sut;

    public DateEntryEditorControllerTests()
    {
        _sut = new DateEntryEditorController(_mediator, _dateSelectionParser);
    }


    [Fact]
    public void Close_ShouldResetAllState()
    {
        // Arrange
        var entry = CreateDateEntryViewModel(
            new DateOnly(2025, 1, 15),
            "Test",
            duration: 3,
            isRecurring: true,
            frequency: 7,
            repetitions: 4);

        _sut.OpenForEdit(entry);

        // Act
        _sut.Close();

        // Assert
        _sut.IsEditorOpen.ShouldBeFalse();
        _sut.IsEditMode.ShouldBeFalse();
        _sut.EditorTitle.ShouldBe("Add Date Entry");
        _sut.EditorDescription.ShouldBeEmpty();
        _sut.EditorStartDateString.ShouldBeNull();
        _sut.EditorDuration.ShouldBe(1);
        _sut.EditorIsRecurring.ShouldBeFalse();
        _sut.EditorFrequency.ShouldBeNull();
        _sut.EditorRepetitions.ShouldBe(0);
        _sut.EditorDetectedSelections.ShouldBeEmpty();
    }


    [Fact]
    public void IsEditorOpen_ShouldRaisePropertyChanged()
    {
        // Arrange
        var raised = false;
        _sut.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(DateEntryEditorController.IsEditorOpen))
                raised = true;
        };

        // Act
        _sut.OpenForDateClick(new DateOnly(2025, 1, 15), []);

        // Assert
        raised.ShouldBeTrue();
    }


    private static DateEntryViewModel CreateDateEntryViewModel(
        DateOnly startDate,
        string description,
        int duration = 1,
        bool isRecurring = false,
        int? frequency = null,
        int repetitions = 0,
        Guid? dateEntryId = null,
        Guid? calendarId = null)
    {
        return new DateEntryViewModel
        {
            DateEntryId = dateEntryId ?? Guid.NewGuid(),
            CalendarId = calendarId ?? Guid.NewGuid(),
            StartDate = startDate,
            Description = description,
            Duration = duration,
            IsRecurring = isRecurring,
            Frequency = frequency,
            NumberOfRepetitions = repetitions
        };
    }


    [Fact]
    public void OpenForSelectedDates_ShouldDoNothing_WhenSelectedDatesIsNull()
    {
        // Act
        _sut.OpenForSelectedDates(null);

        // Assert
        _sut.IsEditorOpen.ShouldBeFalse();
    }

    [Fact]
    public void OpenForSelectedDates_ShouldDoNothing_WhenSelectedDatesIsEmpty()
    {
        // Arrange
        var emptyList = new ArrayList();

        // Act
        _sut.OpenForSelectedDates(emptyList);

        // Assert
        _sut.IsEditorOpen.ShouldBeFalse();
    }

    [Fact]
    public void OpenForSelectedDates_ShouldOpenEditor_WhenSingleDateProvided()
    {
        // Arrange
        var dates = new ArrayList { new DateTime(2025, 1, 15) };
        _dateSelectionParser.ParseSelectedDates(Arg.Any<IEnumerable<DateTime>>())
            .Returns([new DetectedSelection(new DateOnly(2025, 1, 15), 1)]);

        // Act
        _sut.OpenForSelectedDates(dates);

        // Assert
        _sut.IsEditorOpen.ShouldBeTrue();
        _sut.IsEditMode.ShouldBeFalse();
        _sut.EditorTitle.ShouldBe("Add Date Entry");
        _sut.EditorDetectedSelections.Count.ShouldBe(1);
    }

    [Fact]
    public void OpenForSelectedDates_ShouldSetPeriodTitle_WhenConsecutiveDatesProvided()
    {
        // Arrange
        var dates = new ArrayList
        {
            new DateTime(2025, 1, 15),
            new DateTime(2025, 1, 16),
            new DateTime(2025, 1, 17)
        };
        _dateSelectionParser.ParseSelectedDates(Arg.Any<IEnumerable<DateTime>>())
            .Returns([new DetectedSelection(new DateOnly(2025, 1, 15), 3)]);

        // Act
        _sut.OpenForSelectedDates(dates);

        // Assert
        _sut.EditorTitle.ShouldBe("Add Period");
        _sut.EditorDuration.ShouldBe(3);
    }

    [Fact]
    public void OpenForSelectedDates_ShouldSetMultipleEntriesTitle_WhenMultiplePeriodsDetected()
    {
        // Arrange
        var dates = new ArrayList
        {
            new DateTime(2025, 1, 15),
            new DateTime(2025, 1, 17) // Gap creates two selections
        };
        _dateSelectionParser.ParseSelectedDates(Arg.Any<IEnumerable<DateTime>>())
            .Returns([
                new DetectedSelection(new DateOnly(2025, 1, 15), 1),
                new DetectedSelection(new DateOnly(2025, 1, 17), 1)
            ]);

        // Act
        _sut.OpenForSelectedDates(dates);

        // Assert
        _sut.EditorTitle.ShouldBe("Add 2 Date Entries");
        _sut.EditorDetectedSelections.Count.ShouldBe(2);
    }

    [Fact]
    public void OpenForSelectedDates_ShouldResetFormFields()
    {
        // Arrange
        _sut.EditorDescription = "Previous description";
        _sut.EditorIsRecurring = true;
        _sut.EditorFrequency = 7;

        var dates = new ArrayList { new DateTime(2025, 1, 15) };
        _dateSelectionParser.ParseSelectedDates(Arg.Any<IEnumerable<DateTime>>())
            .Returns([new DetectedSelection(new DateOnly(2025, 1, 15), 1)]);

        // Act
        _sut.OpenForSelectedDates(dates);

        // Assert
        _sut.EditorDescription.ShouldBeEmpty();
        _sut.EditorIsRecurring.ShouldBeFalse();
        _sut.EditorFrequency.ShouldBeNull();
        _sut.EditorRepetitions.ShouldBe(0);
    }


    [Fact]
    public void OpenForDateClick_ShouldOpenForEdit_WhenExistingEntryFound()
    {
        // Arrange
        var entry = CreateDateEntryViewModel(
            new DateOnly(2025, 1, 15),
            "Existing Entry",
            duration: 1);

        var entries = new List<DateEntryViewModel> { entry };

        // Act
        _sut.OpenForDateClick(new DateOnly(2025, 1, 15), entries);

        // Assert
        _sut.IsEditorOpen.ShouldBeTrue();
        _sut.IsEditMode.ShouldBeTrue();
        _sut.EditorDescription.ShouldBe("Existing Entry");
    }

    [Fact]
    public void OpenForDateClick_ShouldOpenForAdd_WhenNoExistingEntry()
    {
        // Arrange
        var entries = new List<DateEntryViewModel>();

        // Act
        _sut.OpenForDateClick(new DateOnly(2025, 1, 15), entries);

        // Assert
        _sut.IsEditorOpen.ShouldBeTrue();
        _sut.IsEditMode.ShouldBeFalse();
        _sut.EditorStartDateString.ShouldBe("2025-01-15");
    }

    [Fact]
    public void OpenForDateClick_ShouldFindEntryWithinPeriod()
    {
        // Arrange - Entry spans Jan 15-17
        var entry = CreateDateEntryViewModel(
            new DateOnly(2025, 1, 15),
            "Period Entry",
            duration: 3);

        var entries = new List<DateEntryViewModel> { entry };

        // Act - Click on Jan 16 (middle of period)
        _sut.OpenForDateClick(new DateOnly(2025, 1, 16), entries);

        // Assert
        _sut.IsEditMode.ShouldBeTrue();
        _sut.EditorDescription.ShouldBe("Period Entry");
    }


    [Fact]
    public void OpenForEdit_ShouldPopulateAllFormFields()
    {
        // Arrange
        var entry = CreateDateEntryViewModel(
            new DateOnly(2025, 1, 15),
            "Test Description",
            duration: 5,
            isRecurring: true,
            frequency: 7,
            repetitions: 4);

        // Act
        _sut.OpenForEdit(entry);

        // Assert
        _sut.IsEditorOpen.ShouldBeTrue();
        _sut.IsEditMode.ShouldBeTrue();
        _sut.EditorDescription.ShouldBe("Test Description");
        _sut.EditorStartDateString.ShouldBe("2025-01-15");
        _sut.EditorDuration.ShouldBe(5);
        _sut.EditorIsRecurring.ShouldBeTrue();
        _sut.EditorFrequency.ShouldBe(7);
        _sut.EditorRepetitions.ShouldBe(4);
    }

    [Fact]
    public void OpenForEdit_ShouldSetCorrectTitle_ForSingleDate()
    {
        // Arrange
        var entry = CreateDateEntryViewModel(
            new DateOnly(2025, 1, 15),
            "Single",
            duration: 1);

        // Act
        _sut.OpenForEdit(entry);

        // Assert
        _sut.EditorTitle.ShouldBe("Edit Date Entry");
    }

    [Fact]
    public void OpenForEdit_ShouldSetCorrectTitle_ForPeriod()
    {
        // Arrange
        var entry = CreateDateEntryViewModel(
            new DateOnly(2025, 1, 15),
            "Period",
            duration: 3);

        // Act
        _sut.OpenForEdit(entry);

        // Assert
        _sut.EditorTitle.ShouldBe("Edit Period");
    }

    [Fact]
    public void OpenForEdit_ShouldAddDetectedSelection()
    {
        // Arrange
        var entry = CreateDateEntryViewModel(
            new DateOnly(2025, 1, 15),
            "Test",
            duration: 3);

        // Act
        _sut.OpenForEdit(entry);

        // Assert
        _sut.EditorDetectedSelections.Count.ShouldBe(1);
        _sut.EditorDetectedSelections[0].StartDate.ShouldBe(new DateOnly(2025, 1, 15));
        _sut.EditorDetectedSelections[0].Duration.ShouldBe(3);
    }


    [Fact]
    public async Task SaveAsync_ShouldSendAddRequest_InAddMode()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        _sut.OpenForDateClick(new DateOnly(2025, 1, 15), []);
        _sut.EditorDescription = "New Entry";

        // Act
        await _sut.SaveAsync(calendarId);

        // Assert
        await _mediator.Received(1)
            .Send(
                Arg.Is<AddDateEntryToDbRequest>(r =>
                    r.CalendarId == calendarId &&
                    r.Description == "New Entry" &&
                    r.StartDate == new DateOnly(2025, 1, 15)),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_ShouldSendUpdateRequest_InEditMode()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        var entryId = Guid.NewGuid();
        var entry = CreateDateEntryViewModel(
            new DateOnly(2025, 1, 15),
            "Original",
            duration: 1,
            dateEntryId: entryId,
            calendarId: calendarId);

        _sut.OpenForEdit(entry);
        _sut.EditorDescription = "Updated";

        // Act
        await _sut.SaveAsync(calendarId);

        // Assert
        await _mediator.Received(1)
            .Send(
                Arg.Is<UpdateDateEntryInDbRequest>(r =>
                    r.CalendarId == calendarId &&
                    r.DateEntryId == entryId &&
                    r.Description == "Updated"),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_ShouldCreateMultipleEntries_ForBulkSelection()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        var dates = new ArrayList
        {
            new DateTime(2025, 1, 15),
            new DateTime(2025, 1, 17)
        };
        _dateSelectionParser.ParseSelectedDates(Arg.Any<IEnumerable<DateTime>>())
            .Returns([
                new DetectedSelection(new DateOnly(2025, 1, 15), 1),
                new DetectedSelection(new DateOnly(2025, 1, 17), 1)
            ]);

        _sut.OpenForSelectedDates(dates);
        _sut.EditorDescription = "Bulk Entry";

        // Act
        await _sut.SaveAsync(calendarId);

        // Assert
        await _mediator.Received(2)
            .Send(
                Arg.Any<AddDateEntryToDbRequest>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_ShouldIncludeRecurringFields_WhenIsRecurringTrue()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        _sut.OpenForDateClick(new DateOnly(2025, 1, 15), []);
        _sut.EditorDescription = "Recurring Entry";
        _sut.EditorIsRecurring = true;
        _sut.EditorFrequency = 7;
        _sut.EditorRepetitions = 4;

        // Act
        await _sut.SaveAsync(calendarId);

        // Assert
        await _mediator.Received(1)
            .Send(
                Arg.Is<AddDateEntryToDbRequest>(r =>
                    r.IsRecurring == true &&
                    r.Frequency == 7 &&
                    r.NumberOfRepetitions == 4),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_ShouldExcludeRecurringFields_WhenIsRecurringFalse()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        _sut.OpenForDateClick(new DateOnly(2025, 1, 15), []);
        _sut.EditorDescription = "Non-Recurring";
        _sut.EditorIsRecurring = false;
        _sut.EditorFrequency = 7; // Should be ignored
        _sut.EditorRepetitions = 4; // Should be ignored

        // Act
        await _sut.SaveAsync(calendarId);

        // Assert
        await _mediator.Received(1)
            .Send(
                Arg.Is<AddDateEntryToDbRequest>(r =>
                    r.IsRecurring == false &&
                    r.Frequency == null &&
                    r.NumberOfRepetitions == 0),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_ShouldDoNothing_WhenStartDateIsEmpty()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        _sut.EditorStartDateString = "";

        // Act
        await _sut.SaveAsync(calendarId);

        // Assert
        await _mediator.DidNotReceive()
            .Send(
                Arg.Any<AddDateEntryToDbRequest>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_ShouldDoNothing_WhenStartDateIsInvalid()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        _sut.EditorStartDateString = "not-a-date";

        // Act
        await _sut.SaveAsync(calendarId);

        // Assert
        await _mediator.DidNotReceive()
            .Send(
                Arg.Any<AddDateEntryToDbRequest>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveAsync_ShouldCloseEditor_AfterSuccess()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        _sut.OpenForDateClick(new DateOnly(2025, 1, 15), []);

        // Act
        await _sut.SaveAsync(calendarId);

        // Assert
        _sut.IsEditorOpen.ShouldBeFalse();
    }


    [Fact]
    public async Task DeleteAsync_ShouldSendDeleteRequest_InEditMode()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        var entryId = Guid.NewGuid();
        var entry = CreateDateEntryViewModel(
            new DateOnly(2025, 1, 15),
            "To Delete",
            dateEntryId: entryId,
            calendarId: calendarId);

        _sut.OpenForEdit(entry);

        // Act
        await _sut.DeleteAsync(calendarId);

        // Assert
        await _mediator.Received(1)
            .Send(
                Arg.Is<DeleteDateEntryFromDbRequest>(r =>
                    r.CalendarId == calendarId &&
                    r.DateEntryId == entryId),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldDoNothing_WhenNotInEditMode()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        _sut.OpenForDateClick(new DateOnly(2025, 1, 15), []); // Add mode

        // Act
        await _sut.DeleteAsync(calendarId);

        // Assert
        await _mediator.DidNotReceive()
            .Send(
                Arg.Any<DeleteDateEntryFromDbRequest>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldCloseEditor_AfterSuccess()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        var entry = CreateDateEntryViewModel(
            new DateOnly(2025, 1, 15),
            "To Delete",
            calendarId: calendarId);

        _sut.OpenForEdit(entry);

        // Act
        await _sut.DeleteAsync(calendarId);

        // Assert
        _sut.IsEditorOpen.ShouldBeFalse();
    }


    [Fact]
    public void IsDateWithinEntry_ShouldReturnTrue_ForExactStartDate()
    {
        // Arrange
        var entry = CreateDateEntryViewModel(new DateOnly(2025, 1, 15), "Test", duration: 1);

        // Act
        var result = DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 15), entry);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsDateWithinEntry_ShouldReturnTrue_ForDateWithinPeriod()
    {
        // Arrange
        var entry = CreateDateEntryViewModel(new DateOnly(2025, 1, 15), "Test", duration: 5);

        // Act
        var result = DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 17), entry);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsDateWithinEntry_ShouldReturnTrue_ForExactEndDate()
    {
        // Arrange - Period Jan 15-19 (5 days)
        var entry = CreateDateEntryViewModel(new DateOnly(2025, 1, 15), "Test", duration: 5);

        // Act
        var result = DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 19), entry);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsDateWithinEntry_ShouldReturnFalse_ForDateOutsidePeriod()
    {
        // Arrange
        var entry = CreateDateEntryViewModel(new DateOnly(2025, 1, 15), "Test", duration: 3);

        // Act
        var result = DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 20), entry);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDateWithinEntry_ShouldReturnFalse_ForDateBeforeStart()
    {
        // Arrange
        var entry = CreateDateEntryViewModel(new DateOnly(2025, 1, 15), "Test", duration: 3);

        // Act
        var result = DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 14), entry);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDateWithinEntry_ShouldHandleRecurringEntry()
    {
        // Arrange - Recurring entry starting Jan 15, every 7 days, 2 repetitions
        // Occurrences: Jan 15, Jan 22, Jan 29
        var entry = CreateDateEntryViewModel(
            new DateOnly(2025, 1, 15),
            "Recurring",
            duration: 1,
            isRecurring: true,
            frequency: 7,
            repetitions: 2);

        // Act & Assert
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 15), entry).ShouldBeTrue();
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 22), entry).ShouldBeTrue();
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 29), entry).ShouldBeTrue();
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 20), entry).ShouldBeFalse();
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 2, 5), entry).ShouldBeFalse(); // After last repetition
    }

    [Fact]
    public void IsDateWithinEntry_ShouldHandleRecurringEntryWithDuration()
    {
        // Arrange - Recurring 3-day period starting Jan 15, every 14 days, 1 repetition
        // First occurrence: Jan 15-17
        // Second occurrence: Jan 29-31
        var entry = CreateDateEntryViewModel(
            new DateOnly(2025, 1, 15),
            "Recurring Period",
            duration: 3,
            isRecurring: true,
            frequency: 14,
            repetitions: 1);

        // Act & Assert
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 15), entry).ShouldBeTrue();
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 16), entry).ShouldBeTrue();
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 17), entry).ShouldBeTrue();
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 18), entry).ShouldBeFalse();
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 29), entry).ShouldBeTrue();
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 30), entry).ShouldBeTrue();
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 1, 31), entry).ShouldBeTrue();
        DateEntryEditorController.IsDateWithinEntry(new DateOnly(2025, 2, 1), entry).ShouldBeFalse();
    }
}
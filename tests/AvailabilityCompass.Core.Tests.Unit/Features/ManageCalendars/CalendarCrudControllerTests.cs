using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;
using MediatR;
using NSubstitute;
using Shouldly;

namespace AvailabilityCompass.Core.Tests.Features.ManageCalendars;

public class CalendarCrudControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly CalendarCrudController _sut;

    public CalendarCrudControllerTests()
    {
        _sut = new CalendarCrudController(_mediator);
    }

    [Fact]
    public void ExpandAddCalendar_ShouldSetIsAddCalendarExpandedToTrue()
    {
        // Act
        _sut.ExpandAddCalendar();

        // Assert
        _sut.IsAddCalendarExpanded.ShouldBeTrue();
    }

    [Fact]
    public void ExpandAddCalendar_ShouldInvokeCallback_WhenProvided()
    {
        // Arrange
        var callbackInvoked = false;

        // Act
        _sut.ExpandAddCalendar(() => callbackInvoked = true);

        // Assert
        callbackInvoked.ShouldBeTrue();
    }

    [Fact]
    public void ExpandAddCalendar_ShouldNotThrow_WhenCallbackIsNull()
    {
        // Act & Assert
        Should.NotThrow(() => _sut.ExpandAddCalendar());
    }

    [Fact]
    public void CancelAddCalendar_ShouldResetAllAddFormState()
    {
        // Arrange
        _sut.ExpandAddCalendar();
        _sut.NewCalendarName = "Test Calendar";
        _sut.NewCalendarIsOnly = true;

        // Act
        _sut.CancelAddCalendar();

        // Assert
        _sut.IsAddCalendarExpanded.ShouldBeFalse();
        _sut.NewCalendarName.ShouldBeEmpty();
        _sut.NewCalendarIsOnly.ShouldBeFalse();
    }

    [Fact]
    public async Task AddCalendarAsync_ShouldDoNothing_WhenNameIsEmpty()
    {
        // Arrange
        _sut.NewCalendarName = "";

        // Act
        await _sut.AddCalendarAsync();

        // Assert
        await _mediator.DidNotReceive().Send(Arg.Any<AddCalendarToDbRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddCalendarAsync_ShouldDoNothing_WhenNameIsWhitespace()
    {
        // Arrange
        _sut.NewCalendarName = "   ";

        // Act
        await _sut.AddCalendarAsync();

        // Assert
        await _mediator.DidNotReceive().Send(Arg.Any<AddCalendarToDbRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddCalendarAsync_ShouldSendCorrectRequest()
    {
        // Arrange
        _sut.NewCalendarName = "My Calendar";
        _sut.NewCalendarIsOnly = true;

        // Act
        await _sut.AddCalendarAsync();

        // Assert
        await _mediator.Received(1)
            .Send(
                Arg.Is<AddCalendarToDbRequest>(r =>
                    r.Name == "My Calendar" &&
                    r.IsOnly == true),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddCalendarAsync_ShouldResetStateAfterSuccess()
    {
        // Arrange
        _sut.ExpandAddCalendar();
        _sut.NewCalendarName = "My Calendar";
        _sut.NewCalendarIsOnly = true;

        // Act
        await _sut.AddCalendarAsync();

        // Assert
        _sut.IsAddCalendarExpanded.ShouldBeFalse();
        _sut.NewCalendarName.ShouldBeEmpty();
        _sut.NewCalendarIsOnly.ShouldBeFalse();
    }


    [Fact]
    public void StartCalendarEdit_ShouldSetCorrectState()
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            CalendarId = Guid.NewGuid(),
            Name = "Test Calendar",
            IsOnly = true
        };

        // Act
        _sut.StartCalendarEdit(calendar);

        // Assert
        _sut.IsEditCalendarExpanded.ShouldBeTrue();
        _sut.EditCalendarName.ShouldBe("Test Calendar");
        _sut.EditCalendarIsOnly.ShouldBeTrue();
    }

    [Fact]
    public async Task SaveCalendarEditAsync_ShouldDoNothing_WhenNoEditingCalendar()
    {
        // Arrange - no StartCalendarEdit called

        // Act
        await _sut.SaveCalendarEditAsync();

        // Assert
        await _mediator.DidNotReceive().Send(Arg.Any<UpdateCalendarInDbRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveCalendarEditAsync_ShouldDoNothing_WhenNameIsEmpty()
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            CalendarId = Guid.NewGuid(),
            Name = "Test Calendar",
            IsOnly = false
        };
        _sut.StartCalendarEdit(calendar);
        _sut.EditCalendarName = "";

        // Act
        await _sut.SaveCalendarEditAsync();

        // Assert
        await _mediator.DidNotReceive().Send(Arg.Any<UpdateCalendarInDbRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveCalendarEditAsync_ShouldSendCorrectRequest()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        var calendar = new CalendarViewModel
        {
            CalendarId = calendarId,
            Name = "Original Name",
            IsOnly = false
        };
        _sut.StartCalendarEdit(calendar);
        _sut.EditCalendarName = "Updated Name";
        _sut.EditCalendarIsOnly = true;

        // Act
        await _sut.SaveCalendarEditAsync();

        // Assert
        await _mediator.Received(1)
            .Send(
                Arg.Is<UpdateCalendarInDbRequest>(r =>
                    r.CalendarId == calendarId &&
                    r.Name == "Updated Name" &&
                    r.IsOnly == true),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveCalendarEditAsync_ShouldResetStateAfterSuccess()
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            CalendarId = Guid.NewGuid(),
            Name = "Test Calendar",
            IsOnly = true
        };
        _sut.StartCalendarEdit(calendar);

        // Act
        await _sut.SaveCalendarEditAsync();

        // Assert
        _sut.IsEditCalendarExpanded.ShouldBeFalse();
        _sut.EditCalendarName.ShouldBeEmpty();
        _sut.EditCalendarIsOnly.ShouldBeFalse();
    }

    [Fact]
    public void CancelCalendarEdit_ShouldResetAllEditState()
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            CalendarId = Guid.NewGuid(),
            Name = "Test Calendar",
            IsOnly = true
        };
        _sut.StartCalendarEdit(calendar);

        // Act
        _sut.CancelCalendarEdit();

        // Assert
        _sut.IsEditCalendarExpanded.ShouldBeFalse();
        _sut.EditCalendarName.ShouldBeEmpty();
        _sut.EditCalendarIsOnly.ShouldBeFalse();
    }


    [Fact]
    public void StartDeleteCalendar_ShouldSetConfirmationState()
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            CalendarId = Guid.NewGuid(),
            Name = "Calendar To Delete",
            IsOnly = false
        };

        // Act
        _sut.StartDeleteCalendar(calendar);

        // Assert
        _sut.IsDeleteConfirmationOpen.ShouldBeTrue();
        _sut.DeleteCalendarName.ShouldBe("Calendar To Delete");
    }

    [Fact]
    public async Task ConfirmDeleteCalendarAsync_ShouldDoNothing_WhenNoPendingDelete()
    {
        // Arrange - no StartDeleteCalendar called

        // Act
        await _sut.ConfirmDeleteCalendarAsync();

        // Assert
        await _mediator.DidNotReceive().Send(Arg.Any<DeleteCalendarFromDbRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmDeleteCalendarAsync_ShouldSendDeleteRequest()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        var calendar = new CalendarViewModel
        {
            CalendarId = calendarId,
            Name = "Calendar To Delete",
            IsOnly = false
        };
        _sut.StartDeleteCalendar(calendar);

        // Act
        await _sut.ConfirmDeleteCalendarAsync();

        // Assert
        await _mediator.Received(1)
            .Send(
                Arg.Is<DeleteCalendarFromDbRequest>(r => r.CalendarId == calendarId),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmDeleteCalendarAsync_ShouldResetStateAfterSuccess()
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            CalendarId = Guid.NewGuid(),
            Name = "Calendar To Delete",
            IsOnly = false
        };
        _sut.StartDeleteCalendar(calendar);

        // Act
        await _sut.ConfirmDeleteCalendarAsync();

        // Assert
        _sut.IsDeleteConfirmationOpen.ShouldBeFalse();
        _sut.DeleteCalendarName.ShouldBeEmpty();
    }

    [Fact]
    public void CancelDeleteCalendar_ShouldResetDeleteState()
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            CalendarId = Guid.NewGuid(),
            Name = "Calendar To Delete",
            IsOnly = false
        };
        _sut.StartDeleteCalendar(calendar);

        // Act
        _sut.CancelDeleteCalendar();

        // Assert
        _sut.IsDeleteConfirmationOpen.ShouldBeFalse();
        _sut.DeleteCalendarName.ShouldBeEmpty();
    }

    [Fact]
    public void IsAddCalendarExpanded_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _sut.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(CalendarCrudController.IsAddCalendarExpanded))
                propertyChangedRaised = true;
        };

        // Act
        _sut.ExpandAddCalendar();

        // Assert
        propertyChangedRaised.ShouldBeTrue();
    }

    [Fact]
    public void NewCalendarName_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _sut.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(CalendarCrudController.NewCalendarName))
                propertyChangedRaised = true;
        };

        // Act
        _sut.NewCalendarName = "Test";

        // Assert
        propertyChangedRaised.ShouldBeTrue();
    }

    [Fact]
    public void IsEditCalendarExpanded_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _sut.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(CalendarCrudController.IsEditCalendarExpanded))
                propertyChangedRaised = true;
        };

        // Act
        var calendar = new CalendarViewModel { CalendarId = Guid.NewGuid(), Name = "Test" };
        _sut.StartCalendarEdit(calendar);

        // Assert
        propertyChangedRaised.ShouldBeTrue();
    }

    [Fact]
    public void IsDeleteConfirmationOpen_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _sut.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(CalendarCrudController.IsDeleteConfirmationOpen))
                propertyChangedRaised = true;
        };

        // Act
        var calendar = new CalendarViewModel { CalendarId = Guid.NewGuid(), Name = "Test" };
        _sut.StartDeleteCalendar(calendar);

        // Assert
        propertyChangedRaised.ShouldBeTrue();
    }
}
using System.Reactive.Linq;
using AvailabilityCompass.Core.Features.SearchRecords;
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Search;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.EventBus;
using MediatR;
using NSubstitute;
using Shouldly;

namespace AvailabilityCompass.Core.Tests.Features.SearchRecords;

public class SearchViewModelTests
{
    private readonly ICalendarFilterViewModelFactory _calendarFactory = Substitute.For<ICalendarFilterViewModelFactory>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IEventBus _eventBus = Substitute.For<IEventBus>();
    private readonly IFormElementFactory _formElementFactory = Substitute.For<IFormElementFactory>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ISearchCommandFactory _searchCommandFactory = Substitute.For<ISearchCommandFactory>();
    private readonly ISourceFilterViewModelFactory _sourceFactory = Substitute.For<ISourceFilterViewModelFactory>();

    public SearchViewModelTests()
    {
        _eventBus.ListenToAll().Returns(Observable.Empty<object>());
        _dateTimeProvider.Now.Returns(DateTime.Now);
    }

    [Fact]
    public void WhenCalendarsSectionExpands_ShouldCollapseOtherSections()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.IsSourcesSectionExpanded = true;
        sut.IsFiltersSectionExpanded = true;

        // Act
        sut.IsCalendarsSectionExpanded = true;

        // Assert
        sut.IsSourcesSectionExpanded.ShouldBeFalse();
        sut.IsFiltersSectionExpanded.ShouldBeFalse();
        sut.IsCalendarsSectionExpanded.ShouldBeTrue();
    }

    [Fact]
    public void WhenSourcesSectionExpands_ShouldCollapseOtherSections()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.IsCalendarsSectionExpanded = true;
        sut.IsFiltersSectionExpanded = true;

        // Act
        sut.IsSourcesSectionExpanded = true;

        // Assert
        sut.IsCalendarsSectionExpanded.ShouldBeFalse();
        sut.IsFiltersSectionExpanded.ShouldBeFalse();
        sut.IsSourcesSectionExpanded.ShouldBeTrue();
    }

    [Fact]
    public void WhenFiltersSectionExpands_ShouldCollapseOtherSections()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.IsCalendarsSectionExpanded = true;
        sut.IsSourcesSectionExpanded = true;

        // Act
        sut.IsFiltersSectionExpanded = true;

        // Assert
        sut.IsCalendarsSectionExpanded.ShouldBeFalse();
        sut.IsSourcesSectionExpanded.ShouldBeFalse();
        sut.IsFiltersSectionExpanded.ShouldBeTrue();
    }

    [Fact]
    public void WhenSectionCollapses_ShouldNotAffectOtherSections()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.IsCalendarsSectionExpanded = true;

        // Act
        sut.IsCalendarsSectionExpanded = false;

        // Assert
        sut.IsCalendarsSectionExpanded.ShouldBeFalse();
        sut.IsSourcesSectionExpanded.ShouldBeFalse();
        sut.IsFiltersSectionExpanded.ShouldBeFalse();
    }

    [Fact]
    public void CalendarsSummary_WhenNoCalendarsSelected_ShouldReturnNoneSelected()
    {
        // Arrange
        var sut = CreateViewModel();

        // Act & Assert
        sut.CalendarsSummary.ShouldBe("None selected");
    }

    [Fact]
    public void CalendarsSummary_WhenThreeOrFewerSelected_ShouldShowAllNames()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Calendar1", IsSelected = true });
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Calendar2", IsSelected = true });
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Calendar3", IsSelected = true });

        // Act & Assert
        sut.CalendarsSummary.ShouldBe("Calendar1, Calendar2, Calendar3");
    }

    [Fact]
    public void CalendarsSummary_WhenMoreThanThreeSelected_ShouldShowPlusMore()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Calendar1", IsSelected = true });
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Calendar2", IsSelected = true });
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Calendar3", IsSelected = true });
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Calendar4", IsSelected = true });
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Calendar5", IsSelected = true });

        // Act & Assert
        sut.CalendarsSummary.ShouldBe("Calendar1, Calendar2, Calendar3 +2 more");
    }

    [Fact]
    public void CalendarsSummary_WhenSomeNotSelected_ShouldOnlyCountSelected()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Calendar1", IsSelected = true });
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Calendar2", IsSelected = false });
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Calendar3", IsSelected = true });

        // Act & Assert
        sut.CalendarsSummary.ShouldBe("Calendar1, Calendar3");
    }

    [Fact]
    public void FiltersSummary_WhenAllEmpty_ShouldReturnNoFilters()
    {
        // Arrange
        var sut = CreateViewModel();

        // Act & Assert
        sut.FiltersSummary.ShouldBe("No filters");
    }

    [Fact]
    public void FiltersSummary_WhenSearchPhraseSet_ShouldIncludeQuotedPhrase()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.SearchPhrase = "test search";

        // Act & Assert
        sut.FiltersSummary.ShouldContain("\"test search\"");
    }

    [Fact]
    public void FiltersSummary_WhenStartDateSet_ShouldShowFromDate()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.StartDate = "2025-01-15";

        // Act & Assert
        sut.FiltersSummary.ShouldContain("From: 2025-01-15");
    }

    [Fact]
    public void FiltersSummary_WhenEndDateSet_ShouldShowToDate()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.EndDate = "2025-01-20";

        // Act & Assert
        sut.FiltersSummary.ShouldContain("To: 2025-01-20");
    }

    [Fact]
    public void FiltersSummary_WhenMultipleFiltersSet_ShouldJoinWithPipe()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.SearchPhrase = "test";
        sut.StartDate = "2025-01-15";
        sut.EndDate = "2025-01-20";

        // Act & Assert
        sut.FiltersSummary.ShouldContain(" | ");
    }

    [Fact]
    public void HasFiltersSet_WhenAllEmpty_ShouldReturnFalse()
    {
        // Arrange
        var sut = CreateViewModel();

        // Act & Assert
        sut.HasFiltersSet.ShouldBeFalse();
    }

    [Fact]
    public void HasFiltersSet_WhenSearchPhraseSet_ShouldReturnTrue()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.SearchPhrase = "test";

        // Act & Assert
        sut.HasFiltersSet.ShouldBeTrue();
    }

    [Fact]
    public void HasFiltersSet_WhenStartDateSet_ShouldReturnTrue()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.StartDate = "2025-01-15";

        // Act & Assert
        sut.HasFiltersSet.ShouldBeTrue();
    }

    [Fact]
    public void HasFiltersSet_WhenEndDateSet_ShouldReturnTrue()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.EndDate = "2025-01-20";

        // Act & Assert
        sut.HasFiltersSet.ShouldBeTrue();
    }

    [Fact]
    public void HasCalendarsSelected_WhenNoCalendarsSelected_ShouldReturnFalse()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { IsSelected = false });

        // Act & Assert
        sut.HasCalendarsSelected.ShouldBeFalse();
    }

    [Fact]
    public void HasCalendarsSelected_WhenAnyCalendarSelected_ShouldReturnTrue()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { IsSelected = true });

        // Act & Assert
        sut.HasCalendarsSelected.ShouldBeTrue();
    }

    [Fact]
    public void HasSourcesSelected_WhenNoSourcesSelected_ShouldReturnFalse()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.Sources.Add(new SourceFilterViewModel(_dateTimeProvider) { IsSelected = false });

        // Act & Assert
        sut.HasSourcesSelected.ShouldBeFalse();
    }

    [Fact]
    public void HasSourcesSelected_WhenAnySourceSelected_ShouldReturnTrue()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.Sources.Add(new SourceFilterViewModel(_dateTimeProvider) { IsSelected = true });

        // Act & Assert
        sut.HasSourcesSelected.ShouldBeTrue();
    }

    [Fact]
    public void AvailableDaysSummary_WhenNoAvailableDaysSelected_ShouldReturnEmpty()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { IsSelected = true, IsOnly = false });

        // Act & Assert
        sut.AvailableDaysSummary.ShouldBeEmpty();
    }

    [Fact]
    public void AvailableDaysSummary_WhenAvailableDaysSelected_ShouldShowNames()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Available1", IsSelected = true, IsOnly = true });
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Available2", IsSelected = true, IsOnly = true });

        // Act & Assert
        sut.AvailableDaysSummary.ShouldBe("Available1, Available2");
    }

    [Fact]
    public void BlockedDaysSummary_WhenBlockedDaysSelected_ShouldShowNames()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.Calendars.Add(new CalendarFilterViewModel(Guid.NewGuid()) { Name = "Blocked1", IsSelected = true, IsOnly = false });

        // Act & Assert
        sut.BlockedDaysSummary.ShouldBe("Blocked1");
    }

    [Fact]
    public void HasResults_WhenResultsEmpty_ShouldReturnFalse()
    {
        // Arrange
        var sut = CreateViewModel();

        // Act & Assert
        sut.HasResults.ShouldBeFalse();
    }

    [Fact]
    public void HasResults_WhenResultsNotEmpty_ShouldReturnTrue()
    {
        // Arrange
        var sut = CreateViewModel();
        sut.Results.Add(new Dictionary<string, object> { { "Title", "Test" } });

        // Act & Assert
        sut.HasResults.ShouldBeTrue();
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        var sut = CreateViewModel();

        // Act & Assert
        Should.NotThrow(() => sut.Dispose());
    }

    private SearchViewModel CreateViewModel() => new(
        _mediator,
        _sourceFactory,
        _calendarFactory,
        _formElementFactory,
        _eventBus,
        _searchCommandFactory);
}
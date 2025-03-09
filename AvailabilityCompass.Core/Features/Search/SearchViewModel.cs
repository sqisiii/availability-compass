using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Features.Search.Queries.GetCalendars;
using AvailabilityCompass.Core.Features.Search.Queries.GetSources;
using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using MediatR;

namespace AvailabilityCompass.Core.Features.Search;

public partial class SearchViewModel : ObservableValidator, IPageViewModel
{
    private readonly ICalendarViewModelFactory _calendarViewModelFactory;
    private readonly IMediator _mediator;
    private readonly ISourceViewModelFactory _sourceViewModelFactory;

    private bool _isActive;

    public SearchViewModel(
        IMediator mediator,
        ISourceViewModelFactory sourceViewModelFactory,
        ICalendarViewModelFactory calendarViewModelFactory)
    {
        _mediator = mediator;
        _sourceViewModelFactory = sourceViewModelFactory;
        _calendarViewModelFactory = calendarViewModelFactory;
        _ = LoadSourcesAsync();
        _ = LoadCalendarsAsync();
    }

    public ObservableCollection<SourceViewModel> Sources { get; } = [];

    public ObservableCollection<CalendarViewModel> Calendars { get; } = [];

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value)
            {
                return;
            }

            _isActive = value;
            if (_isActive)
            {
                _ = LoadSourcesAsync();
                _ = LoadCalendarsAsync();
            }
        }
    }

    public string Icon => "SearchWeb";
    public string Name => "Search";

    private async Task LoadSourcesAsync()
    {
        var getSourcesForFilteringDto = await _mediator.Send(new GetSourcesForFilteringQuery());
        Sources.Clear();
        foreach (var source in getSourcesForFilteringDto.Sources)
        {
            var sourceViewModel = _sourceViewModelFactory.Create(source);
            Sources.Add(sourceViewModel);
        }
    }

    private async Task LoadCalendarsAsync()
    {
        var getCalendarsForFilteringDtos = await _mediator.Send(new GetCalendarsForFilteringQuery());
        Calendars.Clear();
        var calendarViewModels = _calendarViewModelFactory.Create(getCalendarsForFilteringDtos);
        foreach (var calendarViewModel in calendarViewModels)
        {
            Calendars.Add(calendarViewModel);
        }
    }
}
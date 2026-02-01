using AvailabilityCompass.Core.Shared.EventBus;
using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords.Search;

/// <summary>
/// Factory for creating SearchCommand instances with the necessary dependencies.
/// </summary>
public class SearchCommandFactory : ISearchCommandFactory
{
    private readonly IEventBus _eventBus;
    private readonly IMediator _mediator;
    private readonly Func<SearchViewModel> _viewModelFactory;

    public SearchCommandFactory(Func<SearchViewModel> viewModelFactory, IMediator mediator, IEventBus eventBus)
    {
        _viewModelFactory = viewModelFactory;
        _mediator = mediator;
        _eventBus = eventBus;
    }

    /// <inheritdoc />
    public ISearchCommand Create()
    {
        return new SearchCommand(_viewModelFactory, _mediator, _eventBus);
    }
}
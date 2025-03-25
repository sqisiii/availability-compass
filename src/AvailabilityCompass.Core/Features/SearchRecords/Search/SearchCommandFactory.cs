using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords.Search;

public class SearchCommandFactory : ISearchCommandFactory
{
    private readonly IMediator _mediator;
    private readonly Func<SearchViewModel> _viewModelFactory;

    public SearchCommandFactory(Func<SearchViewModel> viewModelFactory, IMediator mediator)
    {
        _viewModelFactory = viewModelFactory;
        _mediator = mediator;
    }

    public ISearchCommand Create()
    {
        return new SearchCommand(_viewModelFactory, _mediator);
    }
}
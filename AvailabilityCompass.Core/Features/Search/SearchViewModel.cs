using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Features.Search.Queries.GetSourcesQuery;
using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using MediatR;

namespace AvailabilityCompass.Core.Features.Search;

public partial class SearchViewModel : ObservableValidator, IPageViewModel
{
    private readonly IMediator _mediator;
    private readonly ISourceViewModelFactory _sourceViewModelFactory;

    private bool _isActive;

    public SearchViewModel(IMediator mediator, ISourceViewModelFactory sourceViewModelFactory)
    {
        _mediator = mediator;
        _sourceViewModelFactory = sourceViewModelFactory;
        _ = LoadSourcesAsync();
    }

    public ObservableCollection<SourceViewModel> Sources { get; } = [];

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
            }
        }
    }

    public string Icon => "SearchWeb";
    public string Name => "Search";

    private async Task LoadSourcesAsync()
    {
        var getSourcesResponse = await _mediator.Send(new GetSourcesQuery());
        Sources.Clear();
        foreach (var source in getSourcesResponse.Sources)
        {
            var sourceViewModel = _sourceViewModelFactory.Create(source);
            Sources.Add(sourceViewModel);
        }
    }
}
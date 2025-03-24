using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources.RowerzystaPodroznik;

[SourceService("RowerzystaPodroznik", "Rowerzysta Podróżnik", "PL", false)]
public class RowerzystaPodroznikService : ISourceService
{
    private readonly HttpClient _httpClient;
    private readonly IMediator _mediator;
    private readonly string _sourceId;

    public RowerzystaPodroznikService(HttpClient httpClient, IMediator mediator)
    {
        _httpClient = httpClient;
        _mediator = mediator;
        _sourceId = this.GetSourceId() ?? string.Empty;
    }

    public event EventHandler<SourceRefreshProgressEventArgs>? RefreshProgressChanged;

    public Task<IEnumerable<SourceDataItem>> RefreshSourceDataAsync(CancellationToken ct)
    {
        return Task.FromResult(Enumerable.Empty<SourceDataItem>());
    }

    public Task<List<SourceFilter>> GetFilters(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    private void OnRefreshProgressChanged(double progressPercentage)
    {
        RefreshProgressChanged?.Invoke(this, new SourceRefreshProgressEventArgs(_sourceId, progressPercentage));
    }
}
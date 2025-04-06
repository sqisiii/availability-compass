using AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;
using AvailabilityCompass.Core.Features.ManageSources.Queries.GetFilterOptionsQuery;
using HtmlAgilityPack;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources.ViaVerde;

[SourceService("ViaVerde", "Via Verde", "PL")]
public class ViaVerdeService : ISourceService
{
    private readonly HttpClient _httpClient;
    private readonly IMediator _mediator;
    private readonly string _sourceId;

    public ViaVerdeService(HttpClient httpClient, IMediator mediator)
    {
        _httpClient = httpClient;
        _mediator = mediator;
        _sourceId = this.GetSourceId() ?? string.Empty;
    }

    public event EventHandler<SourceRefreshProgressEventArgs>? RefreshProgressChanged;

    public async Task<IEnumerable<SourceDataItem>> RefreshSourceDataAsync(CancellationToken ct)
    {
        var trips = await ExtractTripsListAsync(ct);
        await _mediator.Send(new ReplaceSourceDataInDbRequest(trips), ct);
        return trips;
    }

    public async Task<List<SourceFilter>> GetFilters(CancellationToken ct)
    {
        List<string> filterFieldNames = [SourceAdditionalDataName.Destination, SourceAdditionalDataName.Type, SourceAdditionalDataName.Remarks];
        var options = await _mediator.Send(new GetFilterOptionsQuery(_sourceId, filterFieldNames), ct);
        List<SourceFilter> filters =
        [
            new SourceFilter
            {
                Label = SourceAdditionalDataName.Destination,
                Type = SourceFilterType.MultiSelect,
                Options = options.FilterOptions.GetValueOrDefault(SourceAdditionalDataName.Destination, [])
            },
            new SourceFilter
            {
                Label = SourceAdditionalDataName.Type,
                Type = SourceFilterType.MultiSelect,
                Options = options.FilterOptions.GetValueOrDefault(SourceAdditionalDataName.Type, [])
            },
            new SourceFilter
            {
                Label = SourceAdditionalDataName.Remarks,
                Type = SourceFilterType.MultiSelect,
                Options = options.FilterOptions.GetValueOrDefault(SourceAdditionalDataName.Remarks, [])
            },
        ];
        return filters;
    }

    private async Task<IReadOnlyCollection<SourceDataItem>> ExtractTripsListAsync(CancellationToken ct)
    {
        var sourceDataItems = new List<SourceDataItem>();
        var counter = 0;
        try
        {
            var html = await _httpClient.GetStringAsync("https://viaverde.com.pl/lista-wypraw/", ct).ConfigureAwait(false);

            var allTripsDoc = new HtmlDocument();
            allTripsDoc.LoadHtml(html);

            var tripsTable = allTripsDoc.DocumentNode.Descendants("table")
                .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("sortable"));

            if (tripsTable is not null)
            {
                var rows = tripsTable.Descendants("tr").Skip(1).ToList(); // Skip header row

                foreach (var row in rows)
                {
                    var cells = row.Descendants("td").ToList();

                    if (cells.Count < 8)
                    {
                        continue;
                    }

                    var tripUrl = cells[0].Descendants("a").FirstOrDefault()?.GetAttributeValue("href", string.Empty) ?? string.Empty;
                    var title = cells[0].InnerText.Trim();
                    var startDate = DateOnly.ParseExact(cells[1].InnerText.Trim(), "yyyy/MM/dd");
                    var endDate = DateOnly.ParseExact(cells[2].InnerText.Trim(), "yyyy/MM/dd");
                    var remarks = cells[3].InnerText.Trim();
                    var price = cells[4].InnerText.Trim();
                    var difficulty = cells[5].InnerText.Trim();
                    var destination = cells[6].InnerText.Trim();
                    var additionalRemarks = cells[7].InnerText.Trim();

                    var type = await GetTypeAsync(tripUrl, ct).ConfigureAwait(false);

                    var tour = new SourceDataItem
                    {
                        SourceId = _sourceId,
                        SeqNo = ++counter,
                        Title = title,
                        Url = tripUrl,
                        StartDate = startDate,
                        EndDate = endDate,
                        ChangeDate = DateTime.Now,
                        AdditionalData = new Dictionary<string, object?>
                        {
                            { SourceAdditionalDataName.Type, type },
                            { SourceAdditionalDataName.Destination, destination },
                            { SourceAdditionalDataName.Price, price },
                            { SourceAdditionalDataName.Remarks, $"{remarks} - {additionalRemarks}" },
                            { SourceAdditionalDataName.Difficulty, difficulty }
                        }
                    };

                    sourceDataItems.Add(tour);
                    OnRefreshProgressChanged((double)counter / rows.Count() * 100);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while parsing data from Barents");
        }

        return sourceDataItems;
    }

    private async Task<string?> GetTypeAsync(string url, CancellationToken ct)
    {
        var html = await _httpClient.GetStringAsync(url, ct).ConfigureAwait(false);

        var tripDoc = new HtmlDocument();
        tripDoc.LoadHtml(html);

        return tripDoc.DocumentNode.Descendants("div")
            .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("winieta"))
            ?.InnerText;
    }

    private void OnRefreshProgressChanged(double progressPercentage)
    {
        RefreshProgressChanged?.Invoke(this, new SourceRefreshProgressEventArgs(_sourceId, progressPercentage));
    }
}
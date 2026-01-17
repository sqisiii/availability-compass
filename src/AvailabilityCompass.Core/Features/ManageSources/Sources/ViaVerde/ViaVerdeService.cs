using HtmlAgilityPack;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources.ViaVerde;

[SourceService("ViaVerde", "Via Verde", "PL", IconFileName = "viaverde.png")]
public sealed class ViaVerdeService : SourceServiceBase
{
    public ViaVerdeService(HttpClient httpClient, IMediator mediator)
        : base(httpClient, mediator)
    {
    }

    protected override IReadOnlyList<string> FilterFieldNames =>
    [
        SourceAdditionalDataName.Destination,
        SourceAdditionalDataName.Type,
        SourceAdditionalDataName.Remarks
    ];

    protected override async Task<IReadOnlyCollection<SourceDataItem>> ExtractSourceDataAsync(CancellationToken ct)
    {
        var sourceDataItems = new List<SourceDataItem>();
        var counter = 0;
        try
        {
            var html = await HttpClient.GetStringAsync("https://viaverde.com.pl/lista-wypraw/", ct).ConfigureAwait(false);

            var allTripsDoc = new HtmlDocument();
            allTripsDoc.LoadHtml(html);

            var tripsTable = allTripsDoc.DocumentNode.Descendants("table")
                .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("sortable"));

            if (tripsTable is not null)
            {
                var rows = tripsTable.Descendants("tr").Skip(1).ToList();

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
                    var transport = cells[3].InnerText.Trim();
                    var price = cells[4].InnerText.Trim();
                    var difficulty = cells[5].InnerText.Trim();
                    var destination = cells[6].InnerText.Trim();
                    var freeSpace = cells[7].InnerText.Trim();

                    var type = await GetTypeAsync(tripUrl, ct).ConfigureAwait(false);

                    const string polishNewPhrase = "Nowość";
                    var isNew = string.Equals(type?.Trim(), polishNewPhrase, StringComparison.OrdinalIgnoreCase);
                    if (isNew)
                    {
                        type = null;
                    }

                    var remarks = string.Empty;
                    if (!string.IsNullOrEmpty(transport))
                    {
                        remarks += transport;
                    }

                    if (!string.IsNullOrEmpty(freeSpace))
                    {
                        if (!string.IsNullOrEmpty(remarks))
                        {
                            remarks += " - ";
                        }

                        remarks += $"{freeSpace}";
                    }

                    var tour = new SourceDataItem
                    {
                        SourceId = SourceId,
                        SeqNo = ++counter,
                        Title = title,
                        Url = tripUrl,
                        StartDate = startDate,
                        EndDate = endDate,
                        ChangeDate = DateTime.Now,
                        AdditionalData = new Dictionary<string, object?>
                        {
                            { SourceAdditionalDataName.Type, type?.Trim() ?? string.Empty },
                            { SourceAdditionalDataName.Destination, destination.Trim() },
                            { SourceAdditionalDataName.Price, price.Trim() },
                            { SourceAdditionalDataName.Remarks, remarks.Trim() },
                            { SourceAdditionalDataName.IsNew, isNew ? "New" : null },
                            { SourceAdditionalDataName.Difficulty, difficulty.Trim() }
                        }
                    };

                    sourceDataItems.Add(tour);
                    ReportProgress((double)counter / rows.Count * 100);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while parsing data from ViaVerde");
        }

        return sourceDataItems;
    }

    private async Task<string?> GetTypeAsync(string url, CancellationToken ct)
    {
        var html = await HttpClient.GetStringAsync(url, ct).ConfigureAwait(false);

        var tripDoc = new HtmlDocument();
        tripDoc.LoadHtml(html);

        return tripDoc.DocumentNode.Descendants("div")
            .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("winieta"))
            ?.InnerText;
    }
}

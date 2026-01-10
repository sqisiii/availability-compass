using System.Globalization;
using AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;
using AvailabilityCompass.Core.Features.ManageSources.Queries.GetFilterOptionsQuery;
using HtmlAgilityPack;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources.Barents;

[SourceService("BarentsPL", "Barents", "PL", IconFileName = "barents.png")]
public class BarentsPlService : ISourceService
{
    private readonly HttpClient _httpClient;
    private readonly IMediator _mediator;
    private readonly string _sourceId;


    public BarentsPlService(HttpClient httpClient, IMediator mediator)
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
        List<string> filterFieldNames =
        [
            SourceAdditionalDataName.Destination, SourceAdditionalDataName.Type, SourceAdditionalDataName.Remarks,
            SourceAdditionalDataName.Difficulty
        ];
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
            new SourceFilter
            {
                Label = SourceAdditionalDataName.Difficulty,
                Type = SourceFilterType.MultiSelect,
                Options = options.FilterOptions.GetValueOrDefault(SourceAdditionalDataName.Difficulty, [])
            }
        ];
        return filters;
    }

    private async Task<IReadOnlyCollection<SourceDataItem>> ExtractTripsListAsync(CancellationToken ct)
    {
        var sourceDataItems = new List<SourceDataItem>();
        var counter = 0;
        try
        {
            var html = await _httpClient.GetStringAsync("https://barents.pl/", ct).ConfigureAwait(false);

            var allTripsDoc = new HtmlDocument();
            allTripsDoc.LoadHtml(html);

            var tripsData = allTripsDoc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Contains("views-row"))
                .Where(node => node.Descendants("div")
                    .Any(div => div.GetAttributeValue("class", "").Contains("destination")))
                .ToList();

            for (var index = 0; index < tripsData.Count; index++)
            {
                var tripData = tripsData[index];
                var title = tripData.Descendants("div")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("description"))
                    ?.InnerText.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault()
                    ?.Trim();

                var destination = tripData.Descendants("div")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("destination"))
                    ?.InnerText;

                var tripUrl = tripData.Descendants("div")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("view-more"))
                    ?.Descendants("a")
                    .FirstOrDefault()
                    ?.GetAttributeValue("href", "");

                var iconFilenames = tripData.Descendants("div")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("icons"))
                    ?.Descendants("img")
                    .Select(img =>
                    {
                        var src = img.GetAttributeValue("src", "");
                        // Extract filename without extension
                        var filename = Path.GetFileNameWithoutExtension(src);
                        return filename;
                    })
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList() ?? new List<string>();

                var isNew = iconFilenames.Contains("nowosc_0");
                var type = iconFilenames.Contains("rower")
                    ? "Biking"
                    : iconFilenames.Contains("trekking")
                        ? "Trekking"
                        : "Other";

                (var parsedSourceDataItems, counter) = await ExtractTripDataAsync($"https://barents.pl{tripUrl}", counter, title, destination, isNew, type, ct);
                sourceDataItems.AddRange(parsedSourceDataItems);
                OnRefreshProgressChanged((double)(index + 1) / tripsData.Count * 100);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while parsing data from Barents PL");
        }

        return sourceDataItems;
    }

    private async Task<(IEnumerable<SourceDataItem> trips, int updatedCounter)> ExtractTripDataAsync(
        string url,
        int counter,
        string? title,
        string? destination,
        bool isNew,
        string? type,
        CancellationToken ct)
    {
        var tours = new List<SourceDataItem>();

        try
        {
            var html = await _httpClient.GetStringAsync(url, ct).ConfigureAwait(false);

            var tripDoc = new HtmlDocument();
            tripDoc.LoadHtml(html);

            var termDivs = tripDoc.DocumentNode.Descendants("div")
                .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("view-termin"));

            if (termDivs != null)
            {
                // Get all table rows (skipping header row)
                var rows = termDivs.Descendants("tr").Skip(1).ToList();

                foreach (var row in rows)
                {
                    // Get all cells in this row
                    var cells = row.Descendants("td").ToList();

                    if (cells.Count < 6)
                    {
                        continue;
                    }

                    var code = cells[0].InnerText.Trim();
                    var difficulty = cells[1].InnerText.Trim();
                    var dateText = cells[2].InnerText.Trim();
                    var price = cells[4].InnerText.Trim();
                    var remarks = cells[5].InnerText.Trim();

                    var dateParts = dateText.Split([" to ", " do "], StringSplitOptions.None);
                    var startDate = DateOnly.MinValue;
                    var endDate = DateOnly.MinValue;

                    if (dateParts.Length == 2)
                    {
                        var startDateText = dateParts[0].Trim();
                        if (startDateText.StartsWith("od ", StringComparison.OrdinalIgnoreCase))
                            startDateText = startDateText[3..];
                        DateOnly.TryParseExact(startDateText, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);
                        DateOnly.TryParseExact(dateParts[1].Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);
                    }

                    var tour = new SourceDataItem
                    {
                        SourceId = _sourceId,
                        SeqNo = ++counter,
                        Title = title ?? "",
                        Url = url,
                        StartDate = startDate,
                        EndDate = endDate,
                        ChangeDate = DateTime.Now,
                        AdditionalData = new Dictionary<string, object?>
                        {
                            { SourceAdditionalDataName.Type, type },
                            { SourceAdditionalDataName.Destination, destination },
                            { SourceAdditionalDataName.Code, code },
                            { SourceAdditionalDataName.Price, price },
                            { SourceAdditionalDataName.IsNew, isNew ? "New" : null },
                            { SourceAdditionalDataName.Remarks, remarks },
                            { SourceAdditionalDataName.Difficulty, difficulty }
                        }
                    };

                    tours.Add(tour);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while extracting trip data from Barents PL");
        }

        return (tours, counter);
    }

    private void OnRefreshProgressChanged(double progressPercentage)
    {
        RefreshProgressChanged?.Invoke(this, new SourceRefreshProgressEventArgs(_sourceId, progressPercentage));
    }
}

using System.Globalization;
using HtmlAgilityPack;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources.Barents;

[SourceService("BarentsEng", "Barents", "ENG", IconFileName = "barents.png")]
public sealed class BarentsEngService : SourceServiceBase
{
    public BarentsEngService(HttpClient httpClient, IMediator mediator)
        : base(httpClient, mediator)
    {
    }

    protected override IReadOnlyList<string> FilterFieldNames =>
    [
        SourceAdditionalDataName.Destination,
        SourceAdditionalDataName.Type,
        SourceAdditionalDataName.Remarks,
        SourceAdditionalDataName.Difficulty
    ];

    protected override async Task<IReadOnlyCollection<SourceDataItem>> ExtractSourceDataAsync(CancellationToken ct)
    {
        var sourceDataItems = new List<SourceDataItem>();
        var counter = 0;
        try
        {
            var html = await HttpClient.GetStringAsync("https://barents.pl/en", ct).ConfigureAwait(false);

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
                        var filename = Path.GetFileNameWithoutExtension(src);
                        return filename;
                    })
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList() ?? [];

                var isNew = iconFilenames.Contains("nowosc_0");
                var type = iconFilenames.Contains("rower")
                    ? "Biking"
                    : iconFilenames.Contains("trekking")
                        ? "Trekking"
                        : "Other";

                (var parsedSourceDataItems, counter) =
                    await ExtractTripDataAsync($"https://barents.pl{tripUrl}", counter, title, destination, isNew, type, ct);
                sourceDataItems.AddRange(parsedSourceDataItems);
                ReportProgress((double)(index + 1) / tripsData.Count * 100);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while parsing data from Barents");
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
            var html = await HttpClient.GetStringAsync(url, ct).ConfigureAwait(false);

            var tripDoc = new HtmlDocument();
            tripDoc.LoadHtml(html);

            var termDivs = tripDoc.DocumentNode.Descendants("div")
                .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("view-termin"));

            if (termDivs != null)
            {
                var rows = termDivs.Descendants("tr").Skip(1).ToList();

                foreach (var row in rows)
                {
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
                        DateOnly.TryParseExact(startDateText, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                            out startDate);
                        DateOnly.TryParseExact(dateParts[1].Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                            out endDate);
                    }

                    var tour = new SourceDataItem
                    {
                        SourceId = SourceId,
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
            Log.Error(e, "Error while extracting trip data from Barents");
        }

        return (tours, counter);
    }
}

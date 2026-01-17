using System.Net;
using HtmlAgilityPack;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources.RowerzystaPodroznik;

[SourceService("RowerzystaPodroznik", "Rowerzysta Podróżnik", "PL", IconFileName = "rowerzystapodroznik.png")]
public sealed class RowerzystaPodroznikService : SourceServiceBase
{
    public RowerzystaPodroznikService(HttpClient httpClient, IMediator mediator)
        : base(httpClient, mediator)
    {
    }

    protected override IReadOnlyList<string> FilterFieldNames =>
        [SourceAdditionalDataName.Destination, SourceAdditionalDataName.Remarks];

    protected override async Task<IReadOnlyCollection<SourceDataItem>> ExtractSourceDataAsync(CancellationToken ct)
    {
        var sourceDataItems = new List<SourceDataItem>();
        var counter = 0;
        try
        {
            var html = await HttpClient.GetStringAsync("https://www.rowerzysta-podroznik.pl/podroze-rowerowe-oferta/", ct).ConfigureAwait(false);

            var allTripsDoc = new HtmlDocument();
            allTripsDoc.LoadHtml(html);

            var tripsData = allTripsDoc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Contains("col-md-6"))
                .ToList();

            for (var index = 0; index < tripsData.Count; index++)
            {
                var tripData = tripsData[index];

                var tripUrl = tripData.Descendants("div")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("thumb"))
                    ?.Descendants("a")
                    .FirstOrDefault()
                    ?.GetAttributeValue("href", "");

                var title = tripData.Descendants("h3")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("post_title"))
                    ?.InnerText.Trim();

                if (title != null)
                {
                    title = WebUtility.HtmlDecode(title);
                }

                var destination = tripData.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("kraj"))
                    ?.InnerText.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault()
                    ?.Trim();

                var price = tripData.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("cena"))
                    ?.InnerText.Trim() + " " + tripData.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("cena2"))
                    ?.InnerText.Trim();

                var date = tripData.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("data"))
                    ?.InnerText.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault()
                    ?.Trim();

                var (startDate, endDate, additionalInfo) = ExtractDate(date);

                if (string.IsNullOrEmpty(tripUrl) || string.IsNullOrEmpty(title))
                {
                    ReportProgress((double)(index + 1) / tripsData.Count * 100);
                    continue;
                }

                (var parsedSourceDataItems, counter) = await ExtractTripDataAsync(tripUrl,
                    counter,
                    title,
                    destination,
                    startDate,
                    endDate,
                    price,
                    additionalInfo,
                    ct);
                sourceDataItems.AddRange(parsedSourceDataItems);
                ReportProgress((double)(index + 1) / tripsData.Count * 100);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while parsing data from RowerzystaPodroznik");
        }

        return sourceDataItems;
    }

    private (DateOnly startDate, DateOnly endDate, string? additionalInfo) ExtractDate(string? fullDate)
    {
        if (string.IsNullOrEmpty(fullDate))
            return (DateOnly.MinValue, DateOnly.MinValue, null);

        var dateParts = fullDate.Split(" - ");
        if (dateParts.Length < 2)
            return (DateOnly.MinValue, DateOnly.MinValue, null);

        var firstPart = dateParts[0].Trim();
        var secondPart = dateParts[1].Trim();

        var startDate = ParseStartDate(firstPart, secondPart);
        var endDate = ParseEndDate(secondPart);
        var additionalInfo = dateParts.Length > 2
            ? string.Join(" - ", dateParts.Skip(2))
            : null;

        return (startDate, endDate, additionalInfo);
    }

    private static DateOnly ParseStartDate(string firstPart, string secondPart)
    {
        // If first part is just a day number, extract month/year from second part
        if (firstPart.All(c => char.IsDigit(c) || char.IsWhiteSpace(c)))
        {
            return ParseDayOnlyWithReference(firstPart, secondPart);
        }

        return PolishDateParser.TryParsePolishDate(firstPart, out var date)
            ? date
            : (DateOnly.TryParse(firstPart, out var fallback) ? fallback : DateOnly.MinValue);
    }

    private static DateOnly ParseDayOnlyWithReference(string dayPart, string referencePart)
    {
        var secondPartWords = referencePart.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (secondPartWords.Length < 2 || !int.TryParse(dayPart.Trim(), out var day))
            return DateOnly.MinValue;

        var month = PolishDateParser.GetMonthNumber(secondPartWords[1]);
        var year = secondPartWords.Length > 2 && int.TryParse(secondPartWords[2], out var y)
            ? y
            : DateTime.Now.Year;

        try
        {
            return new DateOnly(year, month, day);
        }
        catch
        {
            return DateOnly.MinValue;
        }
    }

    private static DateOnly ParseEndDate(string datePart)
    {
        return PolishDateParser.TryParsePolishDate(datePart, out var date)
            ? date
            : (DateOnly.TryParse(datePart, out var fallback) ? fallback : DateOnly.MinValue);
    }

    private async Task<(IEnumerable<SourceDataItem> trips, int updatedCounter)> ExtractTripDataAsync(
        string url,
        int counter,
        string? title,
        string? destination,
        DateOnly startDate,
        DateOnly endDate,
        string price,
        string? additionalInfo,
        CancellationToken ct)
    {
        var tours = new List<SourceDataItem>();

        try
        {
            var html = await HttpClient.GetStringAsync(url, ct).ConfigureAwait(false);

            var tripDoc = new HtmlDocument();
            tripDoc.LoadHtml(html);

            var metaContainer = tripDoc.DocumentNode.Descendants("div")
                .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("trip-meta"));
            var optParagraphs = metaContainer?.Descendants("p")
                .Where(node => node.GetAttributeValue("class", "").Contains("opt"))
                .ToList();

            var type = string.Empty;
            var remarks = string.Empty;
            if (optParagraphs is not null && optParagraphs.Count >= 2)
            {
                const int typeIndex = 0;
                const int remarksIndex = 1;
                type = optParagraphs[typeIndex].InnerText.Trim();
                remarks = optParagraphs[remarksIndex].InnerText.Trim();
            }

            if (!string.IsNullOrEmpty(remarks))
            {
                if (string.IsNullOrEmpty(additionalInfo))
                {
                    additionalInfo = remarks;
                }
                else
                {
                    additionalInfo += " - " + remarks;
                }
            }

            var (condition, technic) = ExtractDifficultyRating(tripDoc);

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
                    { SourceAdditionalDataName.Price, price },
                    { SourceAdditionalDataName.Remarks, additionalInfo },
                    { SourceAdditionalDataName.Difficulty, $"Kondycja: {condition}, Technika: {technic}" }
                }
            };
            tours.Add(tour);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while extracting trip data from RowerzystaPodroznik");
        }

        return (tours, counter);
    }

    private static (double condition, double technic) ExtractDifficultyRating(HtmlDocument doc)
    {
        var wheelsDiv = doc.DocumentNode.Descendants("div")
            .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("wheels"));

        if (wheelsDiv == null)
            return (0, 0);

        var children = wheelsDiv.ChildNodes
            .Where(node => node.Name == "div")
            .ToList();

        double ExtractRating(HtmlNode wheelsChild)
        {
            var fullWheels = wheelsChild.Descendants("span")
                .Count(node => node.GetAttributeValue("class", "").Contains("whell-full"));

            var halfWheels = wheelsChild.Descendants("span")
                .Count(node => node.GetAttributeValue("class", "").Contains("whell-half"));

            return fullWheels + halfWheels * 0.5;
        }

        var conditionRating = children.Count > 0 ? ExtractRating(children[0]) : 0;
        var technicRating = children.Count > 1 ? ExtractRating(children[1]) : 0;

        return (conditionRating, technicRating);
    }
}

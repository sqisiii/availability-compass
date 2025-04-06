using System.Net;
using AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;
using AvailabilityCompass.Core.Features.ManageSources.Queries.GetFilterOptionsQuery;
using HtmlAgilityPack;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources.RowerzystaPodroznik;

[SourceService("RowerzystaPodroznik", "Rowerzysta Podróżnik", "PL")]
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
            var html = await _httpClient.GetStringAsync("https://www.rowerzysta-podroznik.pl/podroze-rowerowe-oferta/", ct).ConfigureAwait(false);

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
                    OnRefreshProgressChanged((double)(index + 1) / tripsData.Count * 100);
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
                OnRefreshProgressChanged((double)(index + 1) / tripsData.Count * 100);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while parsing data from Barents");
        }

        return sourceDataItems;
    }

    private (DateOnly startDate, DateOnly endDate, string? additionalInfo) ExtractDate(string? fullDate)
    {
        var startDate = DateOnly.MinValue;
        var endDate = DateOnly.MinValue;
        string? additionalInfo = null;

        if (string.IsNullOrEmpty(fullDate))
        {
            return (startDate, endDate, additionalInfo);
        }

        var dateParts = fullDate.Split(" - ");
        if (dateParts.Length < 2)
        {
            return (startDate, endDate, additionalInfo);
        }

        var firstPart = dateParts[0].Trim();
        var secondPart = dateParts[1].Trim();

        // If first part doesn't contain month (just day number)
        if (firstPart.All(c => char.IsDigit(c) || char.IsWhiteSpace(c)))
        {
            // Extract month and year from second part
            var secondPartWords = secondPart.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (secondPartWords.Length >= 2)
            {
                var day = int.TryParse(firstPart, out var d) ? d : 1;
                var month = secondPartWords.Length >= 2 ? secondPartWords[1] : string.Empty;
                var year = secondPartWords.Length > 2 && int.TryParse(secondPartWords[2], out _)
                    ? secondPartWords[2]
                    : DateTime.Now.Year.ToString();

                var monthNumber = GetMonthNumber(month);

                try
                {
                    startDate = new DateOnly(int.Parse(year), monthNumber, day);
                }
                catch
                {
                    // Invalid date format, keep default
                }

                if (TryParsePolishDate(secondPart, out var parsedEndDate))
                {
                    endDate = parsedEndDate;
                }
            }
        }
        else
        {
            // Try to extract full dates for both start and end
            if (TryParsePolishDate(firstPart, out var parsedStartDate))
            {
                startDate = parsedStartDate;
            }
            else
            {
                DateOnly.TryParse(firstPart, out startDate);
            }

            if (TryParsePolishDate(secondPart, out var parsedEndDate))
            {
                endDate = parsedEndDate;
            }
            else
            {
                DateOnly.TryParse(secondPart, out endDate);
            }
        }

        // Additional date info if present in more parts
        if (dateParts.Length > 2)
        {
            if (string.IsNullOrEmpty(additionalInfo))
            {
                additionalInfo = string.Join(" - ", dateParts.Skip(2));
            }
            else
            {
                additionalInfo += " - " + string.Join(" - ", dateParts.Skip(2));
            }
        }

        return (startDate, endDate, additionalInfo);
    }

    private bool TryParsePolishDate(string dateText, out DateOnly result)
    {
        result = DateOnly.MinValue;
        if (string.IsNullOrEmpty(dateText))
            return false;

        // Format: "day month year" or "day month"
        var parts = dateText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !int.TryParse(parts[0], out var day))
        {
            return false;
        }

        var month = GetMonthNumber(parts[1]);
        var year = parts.Length > 2 && int.TryParse(parts[2], out var y) ? y : DateTime.Now.Year;

        if (day <= 0 || day > 31 || month <= 0 || month > 12)
        {
            return false;
        }

        try
        {
            result = new DateOnly(year, month, day);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static int GetMonthNumber(string monthName)
    {
        return monthName.ToLower() switch
        {
            "stycznia" or "styczeń" => 1,
            "lutego" or "luty" => 2,
            "marca" or "marzec" => 3,
            "kwietnia" or "kwiecień" => 4,
            "maja" or "maj" => 5,
            "czerwca" or "czerwiec" => 6,
            "lipca" or "lipiec" => 7,
            "sierpnia" or "sierpień" => 8,
            "września" or "wrzesień" => 9,
            "października" or "październik" => 10,
            "listopada" or "listopad" => 11,
            "grudnia" or "grudzień" => 12,
            _ => DateTime.Now.Month
        };
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
            var html = await _httpClient.GetStringAsync(url, ct).ConfigureAwait(false);

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
                    { SourceAdditionalDataName.Price, price },
                    { SourceAdditionalDataName.Remarks, additionalInfo },
                    { SourceAdditionalDataName.Difficulty, $"Kondycja: {condition}, Technika: {technic}" }
                }
            };
            tours.Add(tour);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while extracting trip data from Barents");
        }

        return (tours, counter);
    }

    private (double condition, double technic) ExtractDifficultyRating(HtmlDocument doc)
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

    private void OnRefreshProgressChanged(double progressPercentage)
    {
        RefreshProgressChanged?.Invoke(this, new SourceRefreshProgressEventArgs(_sourceId, progressPercentage));
    }
}
using System.Globalization;
using HtmlAgilityPack;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources.Horyzonty;

/// <summary>
/// Source service for extracting trip data from the Horyzonty travel agency website.
/// </summary>
[SourceService("Horyzonty", "Horyzonty", "PL", IconFileName = "horyzonty.png")]
public sealed class HoryzontyService : SourceServiceBase
{
    public HoryzontyService(HttpClient httpClient, IMediator mediator)
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
            var html = await HttpClient.GetStringAsync("https://www.horyzonty.pl/wyprawy/", ct).ConfigureAwait(false);

            var allTripsDoc = new HtmlDocument();
            allTripsDoc.LoadHtml(html);

            var trips = allTripsDoc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Contains("product__link"))
                .Select(node =>
                {
                    var linkNode = node.Descendants("a").FirstOrDefault();
                    return new
                    {
                        Url = linkNode?.GetAttributeValue("href", ""),
                        IsNew = node.Descendants("span")
                            .Any(span => span.GetAttributeValue("class", "").Contains("product__new"))
                    };
                })
                .Where(trip => !string.IsNullOrEmpty(trip.Url))
                .ToList();

            for (var index = 0; index < trips.Count; index++)
            {
                var trip = trips[index];
                var tripUrl = trip.Url;
                var isNew = trip.IsNew;
                (var parsedSourceDataItems, counter) = await ExtractTripDataAsync($"https://www.horyzonty.pl{tripUrl}",
                    isNew,
                    counter,
                    ct);
                sourceDataItems.AddRange(parsedSourceDataItems);
                ReportProgress((double)(index + 1) / trips.Count * 100);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while parsing data from Horyzonty");
        }

        return sourceDataItems;
    }

    private async Task<(IEnumerable<SourceDataItem> trips, int updatedCounter)> ExtractTripDataAsync(
        string url,
        bool isNew,
        int counter,
        CancellationToken ct)
    {
        var tours = new List<SourceDataItem>();

        try
        {
            var html = await HttpClient.GetStringAsync(url, ct).ConfigureAwait(false);

            var tripDoc = new HtmlDocument();
            tripDoc.LoadHtml(html);

            var title = tripDoc.DocumentNode.Descendants("div")
                .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("hero__title"));

            var destination = tripDoc.DocumentNode.Descendants("span")
                .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("hero__destination"))
                ?.InnerText;

            var type = ExtractCategoryFromUrl(url);

            var labels = tripDoc.DocumentNode.Descendants("label")
                .Where(node => node.GetAttributeValue("class", "").Contains("term__label"));

            foreach (var label in labels)
            {
                var startDate = label.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("term__start"))
                    ?.InnerText;
                var endDate = label.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("term__end"))
                    ?.InnerText;

                var code = label.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("term__code"))
                    ?.InnerText;

                code = code?.Replace("Kod: ", "");

                var price = label.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("term__price"))
                    ?.InnerText;

                var modificator = label.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("term__last"))
                    ?.InnerText;

                counter++;
                var tour = new SourceDataItem(
                    counter,
                    SourceId,
                    title?.InnerText,
                    url,
                    ExtractDateFromText(startDate),
                    ExtractDateFromText(endDate),
                    DateTime.Now,
                    new Dictionary<string, object?>
                    {
                        { SourceAdditionalDataName.Type, type },
                        { SourceAdditionalDataName.Destination, destination },
                        { SourceAdditionalDataName.Code, code },
                        { SourceAdditionalDataName.Price, price },
                        { SourceAdditionalDataName.IsNew, isNew ? "New" : null },
                        { SourceAdditionalDataName.Remarks, modificator ?? string.Empty }
                    }
                );
                tours.Add(tour);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while extracting trip data from Horyzonty");
        }

        return (tours, counter);
    }

    private static DateOnly ExtractDateFromText(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return DateOnly.MinValue;
        }

        var parts = text.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 5)
        {
            return DateOnly.MinValue;
        }

        var dateString = $"{parts[2]} {parts[3]} {parts[4]}";
        var culture = new CultureInfo("pl-PL");
        try
        {
            return DateOnly.ParseExact(dateString, "d MMMM yyyy", culture);
        }
        catch
        {
            return DateOnly.MinValue;
        }
    }

    private static string ExtractCategoryFromUrl(string url)
    {
        var uri = new Uri(url);
        var segments = uri.Segments;
        return segments.Length > 2 ? segments[2].Trim('/') : string.Empty;
    }
}

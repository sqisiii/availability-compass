using System.Globalization;
using AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;
using HtmlAgilityPack;
using MediatR;
using Serilog;

namespace AvailabilityCompass.Core.Features.ManageSources.Integrations.Horyzonty;

public class HoryzontyService : IIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly IMediator _mediator;

    public HoryzontyService(HttpClient httpClient, IMediator mediator)
    {
        _httpClient = httpClient;
        _mediator = mediator;
    }

    //this property is used by Reflection to get the integration name
    public static string IntegrationName => "Horyzonty";

    public static string IntegrationId => "Horyzonty";

    public event EventHandler<SourceRefreshProgressEventArgs>? RefreshProgressChanged;

    public async Task<IEnumerable<SourceDataItem>> RefreshIntegrationDataAsync(CancellationToken ct)
    {
        var trips = await ExtractTripsListAsync(ct);
        await _mediator.Send(new ReplaceSourceDataInDbRequest(trips), ct);
        return trips;
    }

    protected virtual void OnRefreshProgressChanged(double progressPercentage)
    {
        RefreshProgressChanged?.Invoke(this, new SourceRefreshProgressEventArgs(IntegrationId, progressPercentage));
    }

    private async Task<IReadOnlyCollection<SourceDataItem>> ExtractTripsListAsync(CancellationToken ct)
    {
        var sourceDataItems = new List<SourceDataItem>();
        var counter = 0;
        try
        {
            var html = await _httpClient.GetStringAsync("https://www.horyzonty.pl/wyprawy/", ct).ConfigureAwait(false);

            var allTripsDoc = new HtmlDocument();
            allTripsDoc.LoadHtml(html);

            var tripUrls = allTripsDoc.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("class", "").Contains("product__link"))
                .Select(node => node.GetAttributeValue("href", ""))
                .ToList();

            for (var index = 0; index < tripUrls.Count; index++)
            {
                var tripUrl = tripUrls[index];
                (var parsedSourceDataItems, counter) = await ExtractTripDataAsync($"https://www.horyzonty.pl/{tripUrl}", counter, ct);
                sourceDataItems.AddRange(parsedSourceDataItems);
                OnRefreshProgressChanged((double)index / tripUrls.Count * 100);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while parsing data from Horyzonty");
        }

        return sourceDataItems;
    }

    private async Task<(IEnumerable<SourceDataItem> trips, int updatedCounter)> ExtractTripDataAsync(string url, int counter, CancellationToken ct)
    {
        var tours = new List<SourceDataItem>();

        try
        {
            var html = await _httpClient.GetStringAsync(url, ct);

            var tripDoc = new HtmlDocument();
            tripDoc.LoadHtml(html);

            var title = tripDoc.DocumentNode.Descendants("div")
                .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("hero__title"));

            var country = tripDoc.DocumentNode.Descendants("span")
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

                var price = label.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("term__price"))
                    ?.InnerText;

                if (!double.TryParse(price, out var priceValue))
                {
                    priceValue = -1;
                }

                var modificator = label.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("term__last"))
                    ?.InnerText;

                counter++;
                var tour = new SourceDataItem(
                    counter,
                    IntegrationId,
                    title?.InnerText,
                    country,
                    type,
                    ExtractDateFromText(startDate),
                    ExtractDateFromText(endDate),
                    priceValue,
                    DateTime.Now,
                    new Dictionary<string, object?>
                    {
                        { "Code", code },
                        { "Modificator", modificator }
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
        var dateString = $"{parts[2]} {parts[3]} {parts[4]}";
        var culture = new CultureInfo("pl-PL");
        try
        {
            return DateOnly.ParseExact(dateString, "d MMMM yyyy", culture);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return DateOnly.MinValue;
        }
    }

    private static string ExtractCategoryFromUrl(string url)
    {
        var uri = new Uri(url);
        var segments = uri.Segments;
        return segments.Length > 2 ? segments[3].Trim('/') : string.Empty;
    }
}
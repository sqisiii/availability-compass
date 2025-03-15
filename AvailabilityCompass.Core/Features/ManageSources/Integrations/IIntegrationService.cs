namespace AvailabilityCompass.Core.Features.ManageSources.Integrations;

public interface IIntegrationService
{
    Task<IEnumerable<SourceDataItem>> RefreshIntegrationDataAsync(CancellationToken ct);
}
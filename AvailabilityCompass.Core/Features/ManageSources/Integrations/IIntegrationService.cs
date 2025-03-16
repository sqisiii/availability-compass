namespace AvailabilityCompass.Core.Features.ManageSources.Integrations;

public interface IIntegrationService
{
    event EventHandler<SourceRefreshProgressEventArgs> RefreshProgressChanged;

    Task<IEnumerable<SourceDataItem>> RefreshIntegrationDataAsync(CancellationToken ct);
}
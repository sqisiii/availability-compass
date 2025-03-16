namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

public class SourceStore : ISourceStore
{
    private readonly Lazy<IList<SourceData>> _sourceData = new(SourceServiceScanner.ScanSourceServices);
    public IList<SourceData> GetSourceData() => _sourceData.Value;
}
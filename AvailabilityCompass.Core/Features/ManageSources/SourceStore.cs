using AvailabilityCompass.Core.Features.ManageSources.Sources;

namespace AvailabilityCompass.Core.Features.ManageSources;

public class SourceStore : ISourceStore
{
    private readonly Lazy<IList<SourceMetaData>> _sourceMetaData = new(SourceServiceScanner.ScanSourceServices);
    public IList<SourceMetaData> GetSourceMetaData() => _sourceMetaData.Value;
}
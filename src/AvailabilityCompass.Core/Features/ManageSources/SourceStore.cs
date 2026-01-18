using AvailabilityCompass.Core.Features.ManageSources.Sources;

namespace AvailabilityCompass.Core.Features.ManageSources;

/// <summary>
/// Store for managing source metadata, using lazy initialization with reflection-based discovery.
/// </summary>
public class SourceStore : ISourceStore
{
    private readonly Lazy<IList<SourceMetaData>> _sourceMetaData = new(SourceServiceScanner.ScanSourceServices);

    /// <inheritdoc />
    public IList<SourceMetaData> GetSourceMetaData() => _sourceMetaData.Value;
}
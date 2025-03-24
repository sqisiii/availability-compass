namespace AvailabilityCompass.Core.Features.ManageSources;

/// <summary>
/// Represents a store for managing source metadata.
/// </summary>
public interface ISourceStore
{
    /// <summary>
    /// Retrieves a list of source metadata.
    /// </summary>
    /// <returns>A list of <see cref="SourceMetaData"/> objects.</returns>
    IList<SourceMetaData> GetSourceMetaData();
}

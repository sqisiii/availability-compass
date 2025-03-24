namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

/// <summary>
/// Factory interface for creating instances of <see cref="ISourceService"/>.
/// </summary>
public interface ISourceServiceFactory
{
    /// <summary>
    /// Retrieves an instance of <see cref="ISourceService"/> based on the provided key.
    /// </summary>
    /// <param name="key">The key identifying the specific source service.</param>
    /// <returns>An instance of <see cref="ISourceService"/>.</returns>
    ISourceService GetService(string key);
}

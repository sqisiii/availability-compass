namespace AvailabilityCompass.Core.Features.SearchRecords.Search;

/// <summary>
/// Factory interface for creating search command instances.
/// </summary>
public interface ISearchCommandFactory
{
    /// <summary>
    /// Creates a new search command instance.
    /// </summary>
    /// <returns>A new <see cref="ISearchCommand"/> instance.</returns>
    ISearchCommand Create();
}
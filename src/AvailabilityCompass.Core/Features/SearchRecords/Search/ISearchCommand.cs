namespace AvailabilityCompass.Core.Features.SearchRecords.Search;

/// <summary>
/// Defines a command for executing search operations.
/// </summary>
public interface ISearchCommand
{
    /// <summary>
    /// Executes the search command asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous search operation.</returns>
    Task ExecuteAsync();
}
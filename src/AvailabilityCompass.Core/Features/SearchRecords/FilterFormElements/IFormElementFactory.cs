using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetSources;

namespace AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;

/// <summary>
/// Factory interface responsible for creating form elements used as filters in the search records page.
/// </summary>
public interface IFormElementFactory
{
    /// <summary>
    /// Creates a form element group based on the provided source.
    /// </summary>
    /// <param name="source">The source data used to create the form element.</param>
    /// <returns>A <see cref="FormGroup"/> representing the form element.</returns>
    FormGroup CreateFormElement(GetSourcesForFilteringResponse.Source source);
}
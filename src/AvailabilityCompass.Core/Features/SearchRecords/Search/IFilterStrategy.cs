using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;

namespace AvailabilityCompass.Core.Features.SearchRecords.Search;

/// <summary>
/// Defines a strategy for applying filters to source search queries.
/// </summary>
public interface IFilterStrategy
{
    /// <summary>
    /// Applies the filter criteria from the form element to the source filters.
    /// </summary>
    /// <param name="sourceFilters">The source filters to apply the criteria to.</param>
    /// <param name="formElement">The form element containing the filter criteria.</param>
    void ApplyFilter(SearchSourcesQuery.Source sourceFilters, FormElement formElement);
}
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;

namespace AvailabilityCompass.Core.Features.SearchRecords.Search;

/// <summary>
/// Filter strategy for applying multi-select form element values to search queries.
/// </summary>
public class MultiSelectFilterStrategy : IFilterStrategy
{
    /// <inheritdoc />
    public void ApplyFilter(SearchSourcesQuery.Source sourceFilters, FormElement formElement)
    {
        if (formElement.SelectedOptions.Count > 0)
        {
            sourceFilters.SelectedFiltersValues.Add($"sad.{formElement.Label}", formElement.SelectedOptions.ToList());
        }
    }
}
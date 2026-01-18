using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;

namespace AvailabilityCompass.Core.Features.SearchRecords.Search;

/// <summary>
/// Filter strategy for applying text box form element values to search queries.
/// </summary>
public class TextBoxFilterStrategy : IFilterStrategy
{
    /// <inheritdoc />
    public void ApplyFilter(SearchSourcesQuery.Source sourceFilters, FormElement formElement)
    {
        if (!string.IsNullOrEmpty(formElement.TextValue))
        {
            sourceFilters.SelectedFiltersValues.Add($"sad.{formElement.Label}", [formElement.TextValue]);
        }
    }
}
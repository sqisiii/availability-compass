using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;

namespace AvailabilityCompass.Core.Features.SearchRecords.Search;

public class TextBoxFilterStrategy : IFilterStrategy
{
    public void ApplyFilter(SearchSourcesQuery.Source sourceFilters, FormElement formElement)
    {
        if (!string.IsNullOrEmpty(formElement.TextValue))
        {
            sourceFilters.SelectedFiltersValues.Add($"sad.{formElement.Label}", [formElement.TextValue]);
        }
    }
}
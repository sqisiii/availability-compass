using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;

namespace AvailabilityCompass.Core.Features.SearchRecords.Search;

public class MultiSelectFilterStrategy : IFilterStrategy
{
    public void ApplyFilter(SearchSourcesQuery.Source sourceFilters, FormElement formElement)
    {
        if (formElement.SelectedOptions.Count > 0)
        {
            sourceFilters.SelectedFiltersValues.Add($"sad.{formElement.Label}", formElement.SelectedOptions.ToList());
        }
    }
}
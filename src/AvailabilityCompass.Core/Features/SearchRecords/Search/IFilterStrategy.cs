using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;

namespace AvailabilityCompass.Core.Features.SearchRecords.Search;

public interface IFilterStrategy
{
    void ApplyFilter(SearchSourcesQuery.Source sourceFilters, FormElement formElement);
}
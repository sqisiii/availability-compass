using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetSources;

namespace AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;

public interface IFormElementFactory
{
    FormGroup CreateFormElement(GetSourcesForFilteringResponse.Source source);
}
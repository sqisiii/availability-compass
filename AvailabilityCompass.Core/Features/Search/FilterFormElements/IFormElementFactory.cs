using AvailabilityCompass.Core.Features.Search.Queries.GetSources;

namespace AvailabilityCompass.Core.Features.Search.FilterFormElements;

public interface IFormElementFactory
{
    FormGroup CreateFormElement(GetSourcesForFilteringResponse.Source source);
}
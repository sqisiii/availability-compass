using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetSources;

namespace AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;

public class FormElementFactory : IFormElementFactory
{
    public FormGroup CreateFormElement(GetSourcesForFilteringResponse.Source source)
    {
        var formGroup = new FormGroup
        {
            Title = source.Name,
            SourceId = source.SourceId
        };

        foreach (var filter in source.Filters)
        {
            var formElement = new FormElement()
            {
                Label = filter.Label,
                Type = filter.Type switch
                {
                    GetSourcesForFilteringResponse.SourceFilterType.Boolean => FormElementType.CheckBox,
                    GetSourcesForFilteringResponse.SourceFilterType.Text => FormElementType.TextBox,
                    GetSourcesForFilteringResponse.SourceFilterType.MultiSelect => FormElementType.MultiSelect,
                    _ => FormElementType.TextBox
                },
            };
            formElement.Options.AddRange(filter.Options.Select(o => new FormElementSelectOption(o, false)));
            formGroup.Elements.Add(formElement);
        }

        return formGroup;
    }
}
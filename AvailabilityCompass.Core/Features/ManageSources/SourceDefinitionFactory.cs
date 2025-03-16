using AvailabilityCompass.Core.Features.ManageSources.Sources.Horyzonty;

namespace AvailabilityCompass.Core.Features.ManageSources;

public class SourceDefinitionFactory
{
    public SourceDefinition CreateHoryzontyDefinition()
    {
        var sourceDefinition = new SourceDefinition
        {
            Name = "Horyzonty",
            SourceId = HoryzontyService.SourceId,
            IsActive = true,
            ChangedAt = DateTime.Now,
            Filters =
            [
                new SourceFilter
                {
                    Label = "Country",
                    Type = SourceFilterType.MultiSelect
                    // options will be read from database after read from website
                },
                new SourceFilter
                {
                    Label = "Trip Type",
                    Type = SourceFilterType.MultiSelect
                    // options will be read from database after read from website
                },
                new SourceFilter
                {
                    Label = "Availability?",
                    Type = SourceFilterType.MultiSelect,
                    Options = ["", "Normal", "Last places", "Reserve list"]
                }
            ]
        };
        return sourceDefinition;
    }
}
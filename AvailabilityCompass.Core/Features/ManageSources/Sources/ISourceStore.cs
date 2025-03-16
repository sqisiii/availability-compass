namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

public interface ISourceStore
{
    IList<SourceData> GetSourceData();
}
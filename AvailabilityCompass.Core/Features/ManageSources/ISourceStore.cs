namespace AvailabilityCompass.Core.Features.ManageSources;

public interface ISourceStore
{
    IList<SourceData> GetSourceData();
}
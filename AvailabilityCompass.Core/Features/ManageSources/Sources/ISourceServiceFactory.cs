namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

public interface ISourceServiceFactory
{
    ISourceService GetService(string key);
}
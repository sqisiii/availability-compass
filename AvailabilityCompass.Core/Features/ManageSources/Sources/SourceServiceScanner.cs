using System.Reflection;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

public class SourceServiceScanner
{
    public static IList<SourceData> ScanSourceServices()
    {
        var sources = new List<SourceData>();

        var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(ISourceService).IsAssignableFrom(t) && t is
            {
                IsInterface: false,
                IsAbstract: false
            });

        foreach (var type in types)
        {
            var sourceIdProperty = type.GetProperty("SourceId", BindingFlags.Public | BindingFlags.Static);
            if (sourceIdProperty is null || sourceIdProperty.PropertyType != typeof(string))
            {
                continue;
            }

            var sourceId = (string)sourceIdProperty.GetValue(null)!;

            var sourcePropertyName = type.GetProperty("SourceName", BindingFlags.Public | BindingFlags.Static);
            if (sourcePropertyName is null || sourcePropertyName.PropertyType != typeof(string))
            {
                continue;
            }

            var sourceName = (string)sourcePropertyName.GetValue(null)!;

            var sourceEnabledProperty = type.GetProperty("SourceEnabled", BindingFlags.Public | BindingFlags.Static);
            if (sourceEnabledProperty is null || sourceEnabledProperty.PropertyType != typeof(bool))
            {
                continue;
            }

            var sourceEnabled = (bool)sourceEnabledProperty.GetValue(null)!;

            sources.Add(new SourceData(sourceId, sourceName, sourceEnabled));
        }

        return sources;
    }
}
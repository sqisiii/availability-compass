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
            var attr = type.GetCustomAttribute<SourceServiceAttribute>();
            if (attr is null)
            {
                continue;
            }

            sources.Add(new SourceData(attr.Key, attr.Name, attr.IsEnabled));
        }

        return sources;
    }
}
using System.Reflection;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

/// <summary>
/// Scans loaded assemblies for source service implementations decorated with SourceServiceAttribute.
/// </summary>
public class SourceServiceScanner
{
    public static IList<SourceMetaData> ScanSourceServices()
    {
        var sourcesMetaData = new List<SourceMetaData>();

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

            sourcesMetaData.Add(new SourceMetaData(attr.Key, attr.Name, attr.Language, attr.IsEnabled, attr.IconFileName));
        }

        return sourcesMetaData;
    }
}
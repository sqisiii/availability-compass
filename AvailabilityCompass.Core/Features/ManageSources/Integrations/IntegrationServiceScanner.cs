using System.Reflection;

namespace AvailabilityCompass.Core.Features.ManageSources.Integrations;

public class IntegrationServiceScanner
{
    public static Dictionary<string, string> ScanIntegrationServices()
    {
        var integrations = new Dictionary<string, string>();

        var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IIntegrationService).IsAssignableFrom(t) && t is
            {
                IsInterface: false,
                IsAbstract: false
            });

        foreach (var type in types)
        {
            var integrationIdProperty = type.GetProperty("IntegrationId", BindingFlags.Public | BindingFlags.Static);

            if (integrationIdProperty is null || integrationIdProperty.PropertyType != typeof(string))
            {
                continue;
            }

            var integrationId = (string)integrationIdProperty.GetValue(null)!;

            var integrationNameProperty = type.GetProperty("IntegrationName", BindingFlags.Public | BindingFlags.Static);

            if (integrationNameProperty is null || integrationNameProperty.PropertyType != typeof(string))
            {
                continue;
            }

            var integrationName = (string)integrationNameProperty.GetValue(null)!;

            integrations.Add(integrationId, integrationName);
        }

        return integrations;
    }
}
using System.Reflection;

namespace AvailabilityCompass.Core.Features.ManageSources.Integrations;

public class IntegrationServiceScanner
{
    public static IList<IntegrationData> ScanIntegrationServices()
    {
        var integrations = new List<IntegrationData>();

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

            var integrationEnabledProperty = type.GetProperty("IntegrationEnabled", BindingFlags.Public | BindingFlags.Static);
            if (integrationEnabledProperty is null || integrationEnabledProperty.PropertyType != typeof(bool))
            {
                continue;
            }

            var integrationEnabled = (bool)integrationEnabledProperty.GetValue(null)!;

            integrations.Add(new IntegrationData(integrationId, integrationName, integrationEnabled));
        }

        return integrations;
    }
}
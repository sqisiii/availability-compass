namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

/// <summary>
/// Attribute used to define metadata for source services.
/// Applied to classes that implement source service functionality.
/// </summary>
/// <remarks>
/// It is required to apply this attribute to all classes that implement <see cref="ISourceService"/>
/// It is required for reflection-based discovery of source services.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class SourceServiceAttribute : Attribute
{
    public SourceServiceAttribute(string key, string name, string language, bool isEnabled = true)
    {
        Key = key;
        Name = name;
        IsEnabled = isEnabled;
        Language = language;
    }

    public string Key { get; }

    public string Name { get; }

    public bool IsEnabled { get; }

    public string Language { get; }
}

public static class SourceServiceAttributeExtensions
{
    public static string? GetSourceId(this ISourceService sourceService)
    {
        var attr = Attribute.GetCustomAttribute(sourceService.GetType(), typeof(SourceServiceAttribute)) as SourceServiceAttribute;
        return attr?.Key;
    }
}
namespace AvailabilityCompass.Core.Features.ManageSources;

/// <summary>
/// Represents metadata for a data source, including its identifier, display name, language, and enabled status.
/// </summary>
public record SourceMetaData(string Id, string Name, string Language, bool IsEnabled, string IconFileName);
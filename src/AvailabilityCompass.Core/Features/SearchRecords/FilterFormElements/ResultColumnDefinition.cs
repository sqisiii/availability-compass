namespace AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;

/// <summary>
/// Defines a column that will be displayed in search results.
/// </summary>
/// <param name="Header">The display name of the column shown in the UI.</param>
/// <param name="PropertyName">The name of the property to bind to for this column's data.</param>
public record ResultColumnDefinition(string Header, string PropertyName);
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSettings.Queries.GetSetting;

/// <summary>
/// MediatR query to retrieve a setting value by key.
/// </summary>
public class GetSettingQuery : IRequest<GetSettingResponse>
{
    public GetSettingQuery(string key, string? defaultValue = null)
    {
        Key = key;
        DefaultValue = defaultValue;
    }

    public string Key { get; }
    public string? DefaultValue { get; }
}

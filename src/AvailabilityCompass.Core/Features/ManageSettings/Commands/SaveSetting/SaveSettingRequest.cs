using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSettings.Commands.SaveSetting;

/// <summary>
/// MediatR request to save a setting value by key.
/// </summary>
public class SaveSettingRequest : IRequest<SaveSettingResponse>
{
    public SaveSettingRequest(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }
    public string Value { get; }
}

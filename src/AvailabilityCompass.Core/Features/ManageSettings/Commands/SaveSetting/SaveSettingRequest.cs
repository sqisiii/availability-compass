using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSettings.Commands.SaveSetting;

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

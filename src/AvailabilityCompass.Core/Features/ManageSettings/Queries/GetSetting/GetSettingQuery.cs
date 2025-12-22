using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSettings.Queries.GetSetting;

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

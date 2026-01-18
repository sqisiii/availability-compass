namespace AvailabilityCompass.Core.Features.ManageSettings.Queries.GetSetting;

/// <summary>
/// Response containing the retrieved setting value.
/// </summary>
public class GetSettingResponse
{
    public GetSettingResponse(string? value, bool found)
    {
        Value = value;
        Found = found;
    }

    public string? Value { get; }
    public bool Found { get; }
}

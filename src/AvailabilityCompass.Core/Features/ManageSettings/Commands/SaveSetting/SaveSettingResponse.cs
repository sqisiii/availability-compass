namespace AvailabilityCompass.Core.Features.ManageSettings.Commands.SaveSetting;

/// <summary>
/// Response from the save setting command indicating success or failure.
/// </summary>
public class SaveSettingResponse
{
    public SaveSettingResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}

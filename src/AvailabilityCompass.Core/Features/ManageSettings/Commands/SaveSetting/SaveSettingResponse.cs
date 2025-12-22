namespace AvailabilityCompass.Core.Features.ManageSettings.Commands.SaveSetting;

public class SaveSettingResponse
{
    public SaveSettingResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
}

using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageSettings;

/// <summary>
/// View model for managing application settings.
/// </summary>
public partial class ManageSettingsViewModel : ObservableValidator, IPageViewModel
{
    public bool IsActive { get; set; }
    public string Icon => "CogOutline";

    public string Name => "Settings";

    public Task LoadDataAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
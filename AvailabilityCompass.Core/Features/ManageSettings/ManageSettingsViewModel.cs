using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageSettings;

[ObservableRecipient]
public partial class ManageSettingsViewModel : ObservableValidator, IPageViewModel
{
    public string Icon => "CogOutline";

    public string Name => "Settings";

    public Task LoadDataAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
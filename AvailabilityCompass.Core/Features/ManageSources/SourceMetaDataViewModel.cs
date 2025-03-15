using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageSources;

public partial class SourceMetaDataViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime? _changedAt;

    [ObservableProperty]
    private string _integrationId = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private int _tripsCount;
}
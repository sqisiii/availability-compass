using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageSources;

public partial class SourceMetaDataViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime? _changedAt;

    [ObservableProperty]
    private string _integrationId = string.Empty;

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowProgress))]
    private double _progressPercent;

    [ObservableProperty]
    private int _tripsCount;

    public bool ShowProgress => ProgressPercent > 0;
}
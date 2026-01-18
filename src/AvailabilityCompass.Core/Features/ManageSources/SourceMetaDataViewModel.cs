using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageSources;

/// <summary>
/// ViewModel representing metadata for a data source, including refresh progress tracking.
/// </summary>
public partial class SourceMetaDataViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime? _changedAt;

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private string _language = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowProgress))]
    private double _progressPercent;

    [ObservableProperty]
    private string _sourceId = string.Empty;

    [ObservableProperty]
    private int _tripsCount;

    public bool ShowProgress => ProgressPercent > 0;
}
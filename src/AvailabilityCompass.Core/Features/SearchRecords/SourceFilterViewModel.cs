using System.Timers;
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using Timer = System.Timers.Timer;

namespace AvailabilityCompass.Core.Features.SearchRecords;

/// <summary>
/// ViewModel representing a source filter option in the search interface
/// </summary>
public partial class SourceFilterViewModel : ObservableObject, IDisposable
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly Timer _timer;

    [ObservableProperty]
    private FormGroup? _filterFormGroup;

    [ObservableProperty]
    private bool _isActive = true;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _language = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    public SourceFilterViewModel(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        var intervalInMs = 60000;
        _timer = new Timer(intervalInMs); // 1 minute interval
        _timer.Elapsed += OnLastUpdatedTimerElapsed;
        _timer.Start();
    }

    public string HeaderText => $"{Language}  |  {LastUpdated}";

    public string SourceId { get; init; } = string.Empty;

    public DateTime? ChangeAt { get; init; }

    public string IconFileName { get; init; } = string.Empty;

    public string IconPath => string.IsNullOrEmpty(IconFileName) ? string.Empty : $"/Images/Sources/{IconFileName}";

    public string LastUpdated => GetRelativeTime(ChangeAt);

    public bool HasFilters => FilterFormGroup?.Elements.Count > 0;

    public void Dispose()
    {
        _timer.Elapsed -= OnLastUpdatedTimerElapsed;
        _timer.Dispose();
    }

    private void OnLastUpdatedTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        OnPropertyChanged(nameof(LastUpdated));
        OnPropertyChanged(nameof(HeaderText));
    }


    private string GetRelativeTime(DateTime? dateTime)
    {
        if (dateTime is null || dateTime == DateTime.MinValue)
        {
            return "N/A";
        }

        var timeSpan = _dateTimeProvider.Now - dateTime;

        switch (timeSpan.Value.TotalMinutes)
        {
            case < 1:
                return "Just now";
            case < 2:
                return "1 minute ago";
            case < 60:
                return $"{(int)timeSpan.Value.TotalMinutes} minutes ago";
            case < 120:
                return "1 hour ago";
            default:
            {
                if (timeSpan.Value.TotalHours < 24)
                {
                    return $"{(int)timeSpan.Value.TotalHours} hours ago";
                }

                break;
            }
        }

        return timeSpan.Value.TotalDays < 2 ? "1 day ago" : $"{(int)timeSpan.Value.TotalDays} days ago";
    }
}
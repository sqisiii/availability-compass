using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using Timer = System.Timers.Timer;

namespace AvailabilityCompass.Core.Features.SearchRecords;

public partial class SourceFilterViewModel : ObservableObject, IDisposable
{
    private readonly Timer _timer;

    [ObservableProperty]
    private bool _isActive = true;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name = string.Empty;

    public SourceFilterViewModel()
    {
        var intervalInMs = 60000;
        _timer = new Timer(intervalInMs); // 1 minute interval
        _timer.Elapsed += OnLastUpdatedTimerElapsed;
        _timer.Start();
    }

    public string SourceId { get; init; } = string.Empty;

    public DateTime? ChangeAt { get; init; }

    public string LastUpdated => GetRelativeTime(ChangeAt);

    public void Dispose()
    {
        _timer.Elapsed -= OnLastUpdatedTimerElapsed;
        _timer.Dispose();
    }

    private void OnLastUpdatedTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        OnPropertyChanged(nameof(LastUpdated));
    }


    private static string GetRelativeTime(DateTime? dateTime)
    {
        if (dateTime is null || dateTime == DateTime.MinValue)
        {
            return "N/A";
        }

        var timeSpan = DateTime.Now - dateTime;

        switch (timeSpan.Value.TotalMinutes)
        {
            case < 1:
                return "Just now";
            case < 60:
                return $"{(int)timeSpan.Value.TotalMinutes} minutes ago";
            default:
            {
                if (timeSpan.Value.TotalHours < 24)
                {
                    return $"{(int)timeSpan.Value.TotalHours} hours ago";
                }

                break;
            }
        }

        return $"{(int)timeSpan.Value.TotalDays} days ago";
    }
}
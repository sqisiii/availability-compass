using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using Timer = System.Timers.Timer;

namespace AvailabilityCompass.Core.Features.Search;

public partial class SourceViewModel : ObservableObject, IDisposable
{
    private readonly Timer _timer;

    [ObservableProperty]
    private bool _isActive = true;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name = string.Empty;

    public SourceViewModel()
    {
        var intervalInMs = 60000;
        _timer = new Timer(intervalInMs); // 1 minute interval
        _timer.Elapsed += OnLastUpdatedTimerElapsed;
        _timer.Start();
    }

    public DateTime ChangeAt { get; init; }

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


    private static string GetRelativeTime(DateTime dateTime)
    {
        var timeSpan = DateTime.Now - dateTime;

        if (timeSpan.TotalMinutes < 1)
            return "Just now";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} minutes ago";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} hours ago";
        return $"{(int)timeSpan.TotalDays} days ago";
    }
}
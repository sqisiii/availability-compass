namespace AvailabilityCompass.Core.Features.ManageSources;

/// <summary>
/// Event arguments for reporting progress during source data refresh operations.
/// </summary>
public class SourceRefreshProgressEventArgs : EventArgs
{
    public SourceRefreshProgressEventArgs(string sourceId, double progressPercentage)
    {
        SourceId = sourceId;
        ProgressPercentage = progressPercentage;
    }

    public string SourceId { get; }

    public double ProgressPercentage { get; }
}
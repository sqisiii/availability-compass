namespace AvailabilityCompass.Core.Features.ManageSources;

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
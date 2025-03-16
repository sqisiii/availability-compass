namespace AvailabilityCompass.Core.Features.ManageSources;

public class SourceRefreshProgressEventArgs : EventArgs
{
    public SourceRefreshProgressEventArgs(string integrationId, double progressPercentage)
    {
        IntegrationId = integrationId;
        ProgressPercentage = progressPercentage;
    }

    public string IntegrationId { get; }

    public double ProgressPercentage { get; }
}
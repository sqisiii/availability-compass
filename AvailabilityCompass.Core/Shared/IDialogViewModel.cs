namespace AvailabilityCompass.Core.Shared;

public interface IDialogViewModel
{
    bool IsActive { get; set; }

    bool IsDialogOpen { get; set; }
}
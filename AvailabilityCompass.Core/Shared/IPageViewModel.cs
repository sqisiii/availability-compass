namespace AvailabilityCompass.Core.Shared;

public interface IPageViewModel
{
    bool IsActive { get; set; }

    string Icon { get; }

    string Name { get; }
}
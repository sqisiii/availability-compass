using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageSources;

public class ManageSourcesViewModel : ObservableValidator, IPageViewModel
{
    public bool IsActive { get; set; }
    public string Icon => "DatabaseCogOutline";
    public string Name => "Data";
}
using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

public partial class AddRecurringDateViewModel : ObservableObject, IDialogViewModel
{
    public bool IsActive { get; set; }
    public bool IsDialogOpen { get; set; }

    public Task LoadDataAsync()
    {
        return Task.CompletedTask;
    }
}
using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

[ObservableRecipient]
public partial class ManageCalendarsViewModel : ObservableValidator, IPageViewModel
{
    public string Icon => "CalendarClock";
    public string Name => "Calendar";
}
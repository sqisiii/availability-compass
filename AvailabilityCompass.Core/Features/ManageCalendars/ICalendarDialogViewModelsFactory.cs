using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public interface ICalendarDialogViewModelsFactory
{
    AddCalendarViewModel CreateAddCalendarViewModel();
    AddRecurringDateViewModel CreateAddRecurringDateViewModel();
    AddSingleDateViewModel CreateAddSingleDateViewModel();
    UpdateCalendarViewModel CreateUpdateCalendarViewModel();
}
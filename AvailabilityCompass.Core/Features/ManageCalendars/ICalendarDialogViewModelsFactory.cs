using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public interface ICalendarDialogViewModelsFactory
{
    AddCalendarViewModel CreateAddCalendarViewModel();
    DeleteCalendarViewModel CreateDeleteCalendarViewModel();
    UpdateCalendarViewModel CreateUpdateCalendarViewModel();
    AddRecurringDateViewModel CreateAddRecurringDateViewModel();
    AddSingleDateViewModel CreateAddSingleDateViewModel();
    UpdateSingleDateViewModel CreateUpdateSingleDateViewModel();
}
using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public interface ICalendarDialogViewModelsFactory
{
    AddCalendarViewModel CreateAddCalendarViewModel();
    DeleteCalendarViewModel CreateDeleteCalendarViewModel();
    UpdateCalendarViewModel CreateUpdateCalendarViewModel();
    AddRecurringDateViewModel CreateAddRecurringDateViewModel();
    DeleteRecurringDateViewModel CreateDeleteRecurringDateViewModel();
    UpdateRecurringDateViewModel CreateUpdateRecurringDateViewModel();

    AddSingleDateViewModel CreateAddSingleDateViewModel();
    DeleteSingleDateViewModel CreateDeleteSingleDateViewModel();
    UpdateSingleDateViewModel CreateUpdateSingleDateViewModel();
}
using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Factory interface for creating various calendar-related dialog view models.
/// </summary>
public interface ICalendarDialogViewModelsFactory
{
    /// <summary>
    /// Creates an instance of <see cref="AddCalendarViewModel"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="AddCalendarViewModel"/>.</returns>
    AddCalendarViewModel CreateAddCalendarViewModel();

    /// <summary>
    /// Creates an instance of <see cref="DeleteCalendarViewModel"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="DeleteCalendarViewModel"/>.</returns>
    DeleteCalendarViewModel CreateDeleteCalendarViewModel();

    /// <summary>
    /// Creates an instance of <see cref="UpdateCalendarViewModel"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="UpdateCalendarViewModel"/>.</returns>
    UpdateCalendarViewModel CreateUpdateCalendarViewModel();

    /// <summary>
    /// Creates an instance of <see cref="AddRecurringDateViewModel"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="AddRecurringDateViewModel"/>.</returns>
    AddRecurringDateViewModel CreateAddRecurringDateViewModel();

    /// <summary>
    /// Creates an instance of <see cref="DeleteRecurringDateViewModel"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="DeleteRecurringDateViewModel"/>.</returns>
    DeleteRecurringDateViewModel CreateDeleteRecurringDateViewModel();

    /// <summary>
    /// Creates an instance of <see cref="UpdateRecurringDateViewModel"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="UpdateRecurringDateViewModel"/>.</returns>
    UpdateRecurringDateViewModel CreateUpdateRecurringDateViewModel();

    /// <summary>
    /// Creates an instance of <see cref="AddSingleDateViewModel"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="AddSingleDateViewModel"/>.</returns>
    AddSingleDateViewModel CreateAddSingleDateViewModel();

    /// <summary>
    /// Creates an instance of <see cref="DeleteSingleDateViewModel"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="DeleteSingleDateViewModel"/>.</returns>
    DeleteSingleDateViewModel CreateDeleteSingleDateViewModel();

    /// <summary>
    /// Creates an instance of <see cref="UpdateSingleDateViewModel"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="UpdateSingleDateViewModel"/>.</returns>
    UpdateSingleDateViewModel CreateUpdateSingleDateViewModel();
}

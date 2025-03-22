using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;
using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public class CalendarDialogViewModelsFactory : ICalendarDialogViewModelsFactory
{
    private readonly IAbstractFactory<AddCalendarViewModel> _addCalendarViewModelFactory;
    private readonly IAbstractFactory<AddRecurringDateViewModel> _addRecurringDateViewModelFactory;
    private readonly IAbstractFactory<AddSingleDateViewModel> _addSingleDateViewModelFactory;
    private readonly IAbstractFactory<DeleteCalendarViewModel> _deleteCalendarViewModelFactory;
    private readonly IAbstractFactory<UpdateCalendarViewModel> _updateCalendarViewModelFactory;
    private readonly IAbstractFactory<UpdateSingleDateViewModel> _updateSingleDateViewModelFactory;

    public CalendarDialogViewModelsFactory(
        IAbstractFactory<AddCalendarViewModel> addCalendarViewModelFactory,
        IAbstractFactory<DeleteCalendarViewModel> deleteCalendarViewModelFactory,
        IAbstractFactory<UpdateCalendarViewModel> updateCalendarViewModelFactory,
        IAbstractFactory<AddRecurringDateViewModel> addRecurringDateViewModelFactory,
        IAbstractFactory<AddSingleDateViewModel> addSingleDateViewModelFactory,
        IAbstractFactory<UpdateSingleDateViewModel> updateSingleDateViewModelFactory)
    {
        _addCalendarViewModelFactory = addCalendarViewModelFactory;
        _addRecurringDateViewModelFactory = addRecurringDateViewModelFactory;
        _addSingleDateViewModelFactory = addSingleDateViewModelFactory;
        _updateSingleDateViewModelFactory = updateSingleDateViewModelFactory;
        _updateCalendarViewModelFactory = updateCalendarViewModelFactory;
        _deleteCalendarViewModelFactory = deleteCalendarViewModelFactory;
    }

    public AddCalendarViewModel CreateAddCalendarViewModel()
    {
        return _addCalendarViewModelFactory.Create();
    }

    public DeleteCalendarViewModel CreateDeleteCalendarViewModel()
    {
        return _deleteCalendarViewModelFactory.Create();
    }

    public UpdateCalendarViewModel CreateUpdateCalendarViewModel()
    {
        return _updateCalendarViewModelFactory.Create();
    }

    public AddRecurringDateViewModel CreateAddRecurringDateViewModel()
    {
        return _addRecurringDateViewModelFactory.Create();
    }

    public AddSingleDateViewModel CreateAddSingleDateViewModel()
    {
        return _addSingleDateViewModelFactory.Create();
    }

    public UpdateSingleDateViewModel CreateUpdateSingleDateViewModel()
    {
        return _updateSingleDateViewModelFactory.Create();
    }
}
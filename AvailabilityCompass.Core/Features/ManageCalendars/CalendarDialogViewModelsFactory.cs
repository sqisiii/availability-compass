using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;
using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public class CalendarDialogViewModelsFactory : ICalendarDialogViewModelsFactory
{
    private readonly IAbstractFactory<AddCalendarViewModel> _addCalendarViewModelFactory;
    private readonly IAbstractFactory<AddRecurringDateViewModel> _addRecurringDateViewModelFactory;
    private readonly IAbstractFactory<AddSingleDateViewModel> _addSingleDateViewModelFactory;

    public CalendarDialogViewModelsFactory(
        IAbstractFactory<AddCalendarViewModel> addCalendarViewModelFactory,
        IAbstractFactory<AddRecurringDateViewModel> addRecurringDateViewModelFactory,
        IAbstractFactory<AddSingleDateViewModel> addSingleDateViewModelFactory)
    {
        _addCalendarViewModelFactory = addCalendarViewModelFactory;
        _addRecurringDateViewModelFactory = addRecurringDateViewModelFactory;
        _addSingleDateViewModelFactory = addSingleDateViewModelFactory;
    }

    public AddCalendarViewModel CreateAddCalendarViewModel()
    {
        return _addCalendarViewModelFactory.Create();
    }

    public AddRecurringDateViewModel CreateAddRecurringDateViewModel()
    {
        return _addRecurringDateViewModelFactory.Create();
    }

    public AddSingleDateViewModel CreateAddSingleDateViewModel()
    {
        return _addSingleDateViewModelFactory.Create();
    }
}
using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;
using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;
using AvailabilityCompass.Core.Features.SearchRecords;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class CalendarExtensions
{
    public static IServiceCollection AddCalendar(this IServiceCollection services)
    {
        services.AddSingleton<ManageCalendarsViewModel>();
        services.AddSingleton<ICalendarFilterViewModelFactory, CalendarFilterViewModelFactory>();
        services.AddSingleton<ICalendarViewModelFactory, CalendarViewModelFactory>();

        services.AddAbstractFactory<AddCalendarViewModel>();
        services.AddAbstractFactory<DeleteCalendarViewModel>();
        services.AddAbstractFactory<UpdateCalendarViewModel>();
        services.AddAbstractFactory<AddRecurringDateViewModel>();
        services.AddAbstractFactory<DeleteRecurringDateViewModel>();
        services.AddAbstractFactory<UpdateRecurringDateViewModel>();
        services.AddAbstractFactory<AddSingleDateViewModel>();
        services.AddAbstractFactory<UpdateSingleDateViewModel>();
        services.AddAbstractFactory<DeleteSingleDateViewModel>();
        services.AddSingleton<IReservedDatesCalculator, ReservedDatesCalculator>();
        services.AddSingleton<ICalendarDialogViewModelsFactory, CalendarDialogViewModelsFactory>();

        return services;
    }
}
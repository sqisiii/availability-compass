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

        services.AddTransient<IDateSelectionParser, DateSelectionParser>();
        services.AddTransient<ICalendarCrudController, CalendarCrudController>();
        services.AddTransient<IDateEntryEditorController, DateEntryEditorController>();

        services.AddSingleton<ICalendarFilterViewModelFactory, CalendarFilterViewModelFactory>();
        services.AddSingleton<ICalendarViewModelFactory, CalendarViewModelFactory>();

        services.AddAbstractFactory<AddCalendarViewModel>();
        services.AddAbstractFactory<DeleteCalendarViewModel>();
        services.AddAbstractFactory<UpdateCalendarViewModel>();

        services.AddSingleton<IDateProcessor, DateEntryProcessor>();
        services.AddSingleton<IReservedDatesCalculator, ReservedDatesCalculator>();

        return services;
    }
}
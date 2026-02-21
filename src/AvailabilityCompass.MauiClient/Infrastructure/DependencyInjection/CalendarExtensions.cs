using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;
using AvailabilityCompass.Core.Features.SearchRecords;

namespace AvailabilityCompass.MauiClient.Infrastructure.DependencyInjection;

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

        services.AddSingleton<IDateProcessor, DateEntryProcessor>();
        services.AddSingleton<IReservedDatesCalculator, ReservedDatesCalculator>();

        return services;
    }
}
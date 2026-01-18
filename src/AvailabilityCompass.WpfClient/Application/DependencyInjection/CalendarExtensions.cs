using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;
using AvailabilityCompass.Core.Features.ManageCalendars.Dialogs;
using AvailabilityCompass.Core.Features.SearchRecords;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

/// <summary>
/// Extension methods for registering calendar-related services in the DI container.
/// </summary>
public static class CalendarExtensions
{
    /// <summary>
    /// Adds calendar management services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
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
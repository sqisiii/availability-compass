using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.SearchRecords;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class CalendarExtensions
{
    public static IServiceCollection AddCalendar(this IServiceCollection services)
    {
        services.AddSingleton<ManageCalendarsViewModel>();
        services.AddSingleton<ICalendarFilterViewModelFactory, CalendarFilterViewModelFactory>();

        return services;
    }
}
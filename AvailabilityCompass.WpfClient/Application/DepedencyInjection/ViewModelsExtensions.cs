using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageSettings;
using AvailabilityCompass.Core.Features.SelectCriteria;
using AvailabilityCompass.WpfClient.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DepedencyInjection;

public static class ViewModelsExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<ManageSettingsViewModel>();
        services.AddSingleton<ManageCalendarsViewModel>();
        services.AddSingleton<SelectCriteriaViewModel>();
        return services;
    }
}
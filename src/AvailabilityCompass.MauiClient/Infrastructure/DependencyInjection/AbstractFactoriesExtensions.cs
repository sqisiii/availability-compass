using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.MauiClient.Infrastructure.DependencyInjection;

public static class AbstractFactoriesExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAbstractFactory<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddTransient<TInterface, TImplementation>();
            services.AddSingleton<Func<TInterface>>(x => () => x.GetService<TInterface>()!);
            services.AddSingleton<IAbstractFactory<TInterface>, AbstractFactory<TInterface>>();

            return services;
        }

        public IServiceCollection AddAbstractFactory<TImplementation>()
            where TImplementation : class
        {
            services.AddTransient<TImplementation>();
            services.AddSingleton<Func<TImplementation>>(x => () => x.GetService<TImplementation>()!);
            services.AddSingleton<IAbstractFactory<TImplementation>, AbstractFactory<TImplementation>>();

            return services;
        }
    }
}
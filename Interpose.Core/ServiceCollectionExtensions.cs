using Interpose.Core.Generators;
using Interpose.Core.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Interpose.Core
{
    public static class ServiceCollectionExtensions
    {
        private static void AddInterceptedTypeGenerator(IServiceCollection services)
        {
            services.AddSingleton<InterceptedTypeGenerator, RoslynInterceptedTypeGenerator>();
        }

        public static IServiceCollection AddVirtualMethodInterceptor(this IServiceCollection services)
        {
            AddInterceptedTypeGenerator(services);

            services.AddSingleton<ITypeInterceptor, VirtualMethodInterceptor>();

            return services;
        }

        public static IServiceCollection AddInterfaceInterceptor(this IServiceCollection services)
        {
            AddInterceptedTypeGenerator(services);

            services.AddSingleton<IInstanceInterceptor, InterfaceInterceptor>(sp => new InterfaceInterceptor(sp));

            return services;
        }

        public static IServiceCollection AddDynamicInterceptor(this IServiceCollection services)
        {
            AddInterceptedTypeGenerator(services);

            services.AddSingleton<IInstanceInterceptor, DynamicInterceptor>();

            return services;
        }

        public static IServiceCollection AddInterceptedSingleton<TService>(this IServiceCollection services) where TService : class
        {
            AddInterceptedTypeGenerator(services);

            services.AddSingleton<TService>(sp =>
            {
                var type = sp.GetRequiredService<ITypeInterceptor>().InterceptWithAttributes<TService>();
                var proxy = ActivatorUtilities.CreateInstance(sp, type) as TService;
                return proxy;
            });

            return services;
        }

        public static IServiceCollection AddInterceptedTransient<TService>(this IServiceCollection services) where TService : class
        {
            AddInterceptedTypeGenerator(services);

            services.AddTransient<TService>(sp =>
            {
                var type = sp.GetRequiredService<ITypeInterceptor>().InterceptWithAttributes<TService>();
                var proxy = ActivatorUtilities.CreateInstance(sp, type) as TService;
                return proxy;
            });

            return services;
        }

        public static IServiceCollection AddInterceptedScoped<TService>(this IServiceCollection services) where TService : class
        {
            AddInterceptedTypeGenerator(services);

            services.AddScoped<TService>(sp =>
            {
                var type = sp.GetRequiredService<ITypeInterceptor>().InterceptWithAttributes<TService>();
                var proxy = ActivatorUtilities.CreateInstance(sp, type) as TService;
                return proxy;
            });

            return services;
        }

        public static IServiceCollection AddInterceptedSingleton<TService>(this IServiceCollection services, TService target) where TService : class
        {
            AddInterceptedTypeGenerator(services);

            services.AddSingleton<TService>(sp =>
            {
                var proxy = sp.GetRequiredService<IInstanceInterceptor>().InterceptWithAttributes(target) as TService;
                return proxy;
            });

            return services;
        }

        public static IServiceCollection AddInterceptedTransient<TService>(this IServiceCollection services, TService target) where TService : class
        {
            AddInterceptedTypeGenerator(services);

            services.AddTransient<TService>(sp =>
            {
                var proxy = sp.GetRequiredService<IInstanceInterceptor>().InterceptWithAttributes(target) as TService;
                return proxy;
            });

            return services;
        }

        public static IServiceCollection AddInterceptedScoped<TService>(this IServiceCollection services, TService target) where TService : class
        {
            AddInterceptedTypeGenerator(services);

            services.AddScoped<TService>(sp =>
            {
                var proxy = sp.GetRequiredService<IInstanceInterceptor>().InterceptWithAttributes(target) as TService;
                return proxy;
            });

            return services;
        }
    }
}

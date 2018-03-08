using Interpose.Core.Generators;
using Interpose.Core.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Interpose.Core
{
    public static class ServiceCollectionExtensions
    {
        private static void AddInterceptedTypeGenerator(IServiceCollection collection)
        {
            collection.AddSingleton<InterceptedTypeGenerator, RoslynInterceptedTypeGenerator>();
        }

        public static IServiceCollection AddVirtualMethodInterceptor(this IServiceCollection collection)
        {
            AddInterceptedTypeGenerator(collection);

            collection.AddSingleton<ITypeInterceptor, VirtualMethodInterceptor>();

            return collection;
        }

        public static IServiceCollection AddInterfaceInterceptor(this IServiceCollection collection)
        {
            AddInterceptedTypeGenerator(collection);

            collection.AddSingleton<IInstanceInterceptor, InterfaceInterceptor>(sp => new InterfaceInterceptor(sp));

            return collection;
        }

        public static IServiceCollection AddDynamicInterceptor(this IServiceCollection collection)
        {
            AddInterceptedTypeGenerator(collection);

            collection.AddSingleton<IInstanceInterceptor, DynamicInterceptor>();

            return collection;
        }
    }
}

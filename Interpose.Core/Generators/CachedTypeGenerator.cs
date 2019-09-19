using System;
using System.Collections.Generic;
using System.Linq;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Generators
{
    internal sealed class CachedTypeGenerator : InterceptedTypeGenerator, IDisposable
    {
        private readonly InterceptedTypeGenerator generator;
        private readonly Dictionary<string, Type> proxyTypes = new Dictionary<string, Type>();

        public CachedTypeGenerator(InterceptedTypeGenerator generator)
        {
            this.generator = generator;
        }

        public override Type Generate(IInterceptor interceptor, Type baseType, Type handlerType, params Type[] additionalInterfaceTypes)
        {
            var type = this.Lookup(interceptor, baseType, additionalInterfaceTypes, handlerType);

            if (type == null)
            {
                type = this.generator.Generate(interceptor, baseType, handlerType, additionalInterfaceTypes);
                this.Register(interceptor, type, baseType, additionalInterfaceTypes, handlerType);
            }

            return type;
        }

        private void Register(IInterceptor interceptor, Type type, Type baseType, Type[] additionalInterfaceTypes, Type handlerType)
        {
            var key = this.GenerateKey(baseType, additionalInterfaceTypes, handlerType);

            this.proxyTypes[key] = type;
        }

        private string GenerateKey(Type baseType, Type[] additionalInterfaceTypes, Type handlerType)
        {
            var key = string.Join(";", new[] { baseType }.Concat(additionalInterfaceTypes).Concat(new[] { handlerType }).Select(x => x.FullName));

            return key;
        }

        private Type Lookup(IInterceptor interceptor, Type baseType, Type[] additionalInterfaceTypes, Type handlerType)
        {
            var key = this.GenerateKey(baseType, additionalInterfaceTypes, handlerType);

            this.proxyTypes.TryGetValue(key, out var proxyType);

            return proxyType;
        }

        public void Dispose()
        {
            this.proxyTypes.Clear();
        }
    }
}

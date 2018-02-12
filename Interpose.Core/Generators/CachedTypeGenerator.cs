using System;
using Interpose.Core.Interceptors;

namespace Interpose.Core.Generators
{
    public sealed class CachedTypeGenerator : InterceptedTypeGenerator
    {
        private readonly InterceptedTypeGenerator generator;

        public CachedTypeGenerator(InterceptedTypeGenerator generator)
        {
            this.generator = generator;
        }

        public override Type Generate(IInterceptor interceptor, Type baseType, Type handlerType, params Type[] additionalInterfaceTypes)
        {
            var type = this.Lookup(baseType, additionalInterfaceTypes, handlerType);

            if (type == null)
            {
                type = this.generator.Generate(interceptor, baseType, handlerType, additionalInterfaceTypes);
                this.Register(type, baseType, additionalInterfaceTypes, handlerType);
            }

            return type;
        }

        private void Register(Type type, Type baseType, Type[] additionalInterfaceTypes, Type handlerType)
        {
            throw new NotImplementedException();
        }

        private Type Lookup(Type baseType, Type[] additionalInterfaceTypes, Type handlerType)
        {
            throw new NotImplementedException();
        }
    }
}

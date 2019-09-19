using Interpose.Core.Generators;
using Interpose.Core.Handlers;
using System;

namespace Interpose.Core.Interceptors
{
    /// <summary>
    /// Intercepts non-sealed classes with virtual methods.
    /// </summary>
	public sealed class VirtualMethodInterceptor : ITypeInterceptor
	{
		private readonly InterceptedTypeGenerator generator;

		public VirtualMethodInterceptor(InterceptedTypeGenerator generator)
		{
			this.generator = generator;
		}

		public VirtualMethodInterceptor() : this(RoslynInterceptedTypeGenerator.Instance)
		{

		}

		private Type CreateType(Type typeToIntercept, Type handlerType)
		{
			return this.generator.Generate(this, typeToIntercept, handlerType);
		}

		public Type Intercept(Type typeToIntercept, Type handlerType)
		{
			if (typeToIntercept == null)
			{
				throw new ArgumentNullException(nameof(typeToIntercept));
			}

			if (handlerType == null)
			{
				throw new ArgumentNullException(nameof(handlerType));
			}

			if (this.CanIntercept(typeToIntercept) == false)
			{
				throw new ArgumentException("Type to intercept is not interceptable by this interceptor", nameof(typeToIntercept));
			}

			if (typeof(IInterceptionHandler).IsAssignableFrom(handlerType) == false)
			{
				throw new ArgumentException("Handler type is not an IInterceptionHandler", nameof(handlerType));
			}

			if (handlerType.IsPublic == false)
			{
				throw new ArgumentException("Handler type is not public", nameof(handlerType));
			}

			if ((handlerType.IsAbstract == true) || (handlerType.IsInterface == true))
			{
				throw new ArgumentException("Handler type is not a concrete class", nameof(handlerType));
			}

			if (handlerType.GetConstructor(Type.EmptyTypes) == null)
			{
				throw new ArgumentException("Handler type does not have a public parameterless constructor", nameof(handlerType));
			}

			return this.CreateType(typeToIntercept, handlerType);
		}

		public bool CanIntercept(Type typeToIntercept)
		{
			return (typeToIntercept.IsInterface == false) && (typeToIntercept.IsSealed == false);
		}
	}
}

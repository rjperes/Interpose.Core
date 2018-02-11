using Interpose.Core.Handlers;
using Interpose.Core.Interceptors;
using Interpose.Core.Proxies;
using System;
using Xunit;

namespace Interpose.Core.Tests
{
	public class InterceptionTests
	{
		[Fact]
		public void CanDoDynamicInterception()
		{
			//Dynamic interceptor
			var instance = new MyType();
			var interceptor = new DynamicInterceptor();
			var handler = new MyHandler();
			var canIntercept = interceptor.CanIntercept(instance);
			dynamic myProxy = interceptor.Intercept(instance, null, handler);
			var proxy = myProxy as IInterceptionProxy;
			var otherInterceptor = proxy.Interceptor;
			int result = myProxy.MyMethod();
			Assert.Equal(20, result);
		}

		[Fact]
		public void CanDoInterfaceInterception()
		{
			//Interface interceptor
			var instance = new MyType();
			var type = typeof(IMyType);
			var interceptor = new InterfaceInterceptor();
			var handler = new MyHandler();
			var canIntercept = interceptor.CanIntercept(instance);
			var myProxy = interceptor.Intercept(instance, type, handler) as IMyType;
			var proxy = myProxy as IInterceptionProxy;
			var otherInterceptor = proxy.Interceptor;
			var result = myProxy.MyMethod();
			Assert.Equal(20, result);
		}

		[Fact]
		public void CanDoVirtualInterception()
		{
			//Virtual method interceptor
			var type = typeof(MyType);
			var handlerType = typeof(MyHandler);
			var interceptor = new VirtualMethodInterceptor();
			var canIntercept = interceptor.CanIntercept(type);
			var myProxyType = interceptor.Intercept(type, handlerType);
			var myProxy = Activator.CreateInstance(myProxyType) as IMyType;
			var result = myProxy.MyMethod();
			Assert.Equal(20, result);
		}

		[Fact]
		public void CanDoDynamicInterceptionWithAttributes()
		{
			var instance = new MyType3();
			dynamic myProxy = DynamicInterceptor.Instance.InterceptWithAttributes(instance);
			myProxy.MyMethod();
		}

		[Fact]
		public void CanDoDynamicInterceptionWithRegistry()
		{
			var instance = new MyType3();
			var interceptor = new DynamicInterceptor();
			var registry = new RegistryInterceptionHandler();
			registry.Register<IMyType>(x => x.MyProperty, new MyHandler());
			registry.Register<IMyType>(x => x.MyMethod(), new MyHandler());
			dynamic myProxy = interceptor.Intercept(instance, null, registry);
			myProxy.MyMethod();
		}
	}
}

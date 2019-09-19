using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Interpose.Core.Interceptors;
using Microsoft.Extensions.Caching.Memory;

namespace Interpose.Core.Handlers
{

    public sealed class CachingInterceptionHandler : IInterceptionHandler
    {
        private static readonly Guid KeyGuid = new Guid("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE");
        private readonly TimeSpan duration;
        private readonly IMemoryCache memoryCache;

        public CachingInterceptionHandler(IMemoryCache memoryCache, TimeSpan duration)
        {
            this.duration = duration;
            this.memoryCache = memoryCache;
        }

        public void Invoke(InterceptionArgs arg)
        {
            var key = this.CreateCacheKey(arg.Method, arg.Arguments);

            if (this.GetCachedResult(key, out var result) == false)
            {
                arg.Proceed();
                result = arg.Result;
                this.SetCacheResult(key, result);
            }
            else
            {
                arg.Result = result;
            }
        }

        private void SetCacheResult(string key, object result)
        {
            this.memoryCache.Set(key, result);
        }

        private bool GetCachedResult(string key, out object result)
        {
            result = this.memoryCache.Get(key);

            return result != null;
        }

        private string CreateCacheKey(MethodBase method, params object[] inputs)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}:", Process.GetCurrentProcess().Id);
            sb.AppendFormat("{0}:", KeyGuid);

            if (method.DeclaringType != null)
            {
                sb.Append(method.DeclaringType.FullName);
            }

            sb.Append(':');
            sb.Append(method);

            foreach (var input in inputs ?? new object[0])
            {
                sb.Append(':');
                sb.Append(input?.GetHashCode());
            }

            return sb.ToString();
        }
    }
}

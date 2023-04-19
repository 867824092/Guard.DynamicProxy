using System;
using System.Diagnostics;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.DependencyInjection.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Guard.DynamicProxy.DependencyInjection {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [DebuggerDisplay("ServiceInterceptor: Type={ServiceType} Order={Order}")]
    public class ServiceInterceptorAttribute : Attribute  {
        public ServiceInterceptorAttribute(Type type)
        {
            ServiceType = type ?? throw new ArgumentNullException(nameof(type));
        }
        public int Order { get; set; }
        public Type ServiceType { get; }

        public IInterceptor CreateInstance(IServiceProvider serviceProvider) {
            if (serviceProvider == null) {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            return (IInterceptor)serviceProvider.GetRequiredService(ServiceType);
        }
    }
}
